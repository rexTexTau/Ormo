# Ormo
⚡ Lightning-fast 📍 extra-tiny ORM in 🍴 CQRS style.

Does not make any SQL code generation (which leads to faster execution speed and more ability to fine-tune and control by develolper), but provides a convenient way to create new scripts, use existing SQL scripts and link with data classes on C# side.

# Usage
A picture is worth thousand words:

![alt text](https://raw.githubusercontent.com/rexTexTau/Ormo/refs/heads/main/illus.png)

Just
1. Write your parametrized SQL queries and commands
2. Copy SQL files to `Query` and `Command` (name matters) folders in your solution
3. Place command and query classes right in these folders, named exactly equal to SQL script name, that this class should use (this is how Ormo works)
4. Add [*EmbeddedResourcesScriptProvider*](https://github.com/rexTexTau/Ormo/blob/main/Ormo//ScriptProviders/EmbeddedResourcesScriptProvider.cs) (in ths case, SQL files should have **Build action** = *Embedded Resource*) or [*FolderScriptProvider*](https://github.com/rexTexTau/Ormo/blob/main/Ormo//ScriptProviders/FolderScriptProvider.cs) (in this case, SQL files should have **Build action** = *Content* and **Copy to OutputDirectory** = *Copy always* or *Copy if newer*) to your Ormo consumer project
5. Enjoy (just call Query's or Command's *RunAsync* or *Run* method, and get strongly-typed results)

# Sample code
## QuerySingle implementation
[*QuerySingle*](https://github.com/rexTexTau/Ormo/blob/main/Ormo/QuerySingle.cs) base class should be used for queries that return a single data row or ordinal data.

*Sample Query class*:
```
public sealed class GetLastResultByWorker : QuerySingle<int, JobInfo>
{
    public GetResultsByWorker(IScriptProvider scriptsProvider) : base(scriptsProvider) { }
}
```

First generic type parameter is a query parameter (in this sample, just an integer), and second generic type parameter describes the query result (*JobInfo* here in this sample is just a POCO model).

That's it! Just four lines of code, and you have a query class, which, being paired with corresponding SQL, returns strongly-typed data (by calling its *Run* or *RunAsync* method from base query class).

*Sample Query SQL*:
```
SELECT id, succeeded, status, data, acquired
FROM result
WHERE worker_id = @param
ORDER BY acquired DESC
LIMIT 1;
```

*Sample calling code*:
```
var query = new GetLastResultByWorker(/*instance of the script provider*/);
query.Setup(1); // setting the query parameters using base Setup method
var result = await query.RunAsync(/*connection to the database*/);
```

You can reuse the query class, calling *Setup* methods setting query parameters' values you need before calling another *Run* or *RunAsync*.

## QueryMultiple implementation
[*QueryMultiple*](https://github.com/rexTexTau/Ormo/blob/main/Ormo/QueryMultiple.cs) base class should be used for queries that return multiple resulting rows of data. 

*Sample Query class*:
```
public sealed class GetAllJobs : QueryMultiple<Nothing, JobInfo>
{
	public GetAllJobs(IScriptProvider scriptsProvider) : base(scriptsProvider) { }

	protected override JobInfo RecordProcessor(DbDataReader reader)
	{
		return new JobInfo(
			reader.GetInt32(reader.GetOrdinal("id")),
			reader.GetString(reader.GetOrdinal("type")),
			reader.GetString(reader.GetOrdinal("name")),
			reader.GetBoolean(reader.GetOrdinal("active")),
			reader.GetString(reader.GetOrdinal("state")),
			reader.GetString(reader.GetOrdinal("next_run")));
	}
}
```

In this case, query has no input parameters, so special struct [*Nothing*](https://github.com/rexTexTau/Ormo/blob/main/Ormo/Nothing.cs) is passed as TP generic parameter to specify it.

*JobInfo* here is just a POCO model with a constructor (that is called from *RecordProcessor*) and a set of corresponding properties to store result.

Overriding *RecordProcessor* is quite optional – Ormo automatically fills in resulting class' fields by name (**type** will be translated to **Type**, **next_run** will be translated to **NextRun**, etc). You have to override it only if you have speed concerns (Ormo uses RTTI to do fields mapping, but your code can avoid it the same way it was done here in the sample).

*Sample Query SQL*:
```
SELECT worker.id, worker.type, worker.name, worker.active, job.state, job.next_run
FROM worker
LEFT JOIN job
ON worker.id = job.worker_id;
```

*RunAsync* method of [*QueryMultiple*](https://github.com/rexTexTau/Ormo/blob/main/Ormo/QueryMultiple.cs) class provides the result in form of IAsyncEnumerable, that makes it pretty handy to iterate over.

*Sample calling code*:
```
foreach (var job in await new GetAllJobs(/*instance of the script provider*/).RunAsync(/*connection to the database*/))
{
	// business logic
};
```

Cause we do not need to call *Setup* in the absence of query parameters, the whole query call could easily be a one-liner.

## Command implementation
[*Command*](https://github.com/rexTexTau/Ormo/blob/main/Ormo/Command.cs) base class should be used for commands. 

*Sample Command class*:
```
public class StoreRunResult : Command<WorkerRunResult>
{
	public StoreRunResult(IScriptProvider provider, IClassToDatabaseFieldNameConverter? fieldNameConverter = null) : base(provider, fieldNameConverter)
	{
	}

	public override StoreRunResult Setup(RunResult data)
	{
		Parameters = new Dictionary<string, object>
		{
			{ "id", data.WorkerId },
			{ "active", data.Active },
			{ "state", data.NeedsInteraction ? "NeedsInteraction" : "Waiting" },
			{ "data", NullToDbNull(data.Data) },
			{ "succeeded", data.Succeeded },
			{ "elapsed", data.Elapsed },
			{ "next_run_in", ((int)data.NextRunIn.TotalSeconds).ToString() }
		};
		return this;
	}

	protected override bool RowsAffectedProcessor(int rowsAffected)
	{
		return rowsAffected >= 3;
	}
}
```

*RunResult* here is just a POCO model with a constructor (that is called from *RecordProcessor*) and a set of corresponding properties to store result.

Command does not provide any result, other than number of rows, affected by the command query – so, you have a special *RowsAffectedProcessor* method to override: this method should determine, based on `rowsAffected` number, was the command execution successful or not (base method always returns *true*).

You can also provide a typed override of *Setup* base method to enable method chaining support (that makes it possible to make a query or command call a one-liner, even with a need to call *Setup*). Also overriding this method can speed up the code (omitting dealing with RTTI that Ormo does) and handle some business logic inside (as shown in this sample) – just fill in *Parameters* dictionary provided by base class.

*Sample calling code*:
```
var success = await new StoreRunResult(/*instance of the script provider*/).Setup(dataToStore).Run(/*connection to the database*/)
```

Cause Ormo uses parametrized SQL scripts to operate, you can make additional data transformations and/or calculations in the script, alter multiple tables, use transactions etc – almost no limits (as shown here):

*Sample Command SQL*:
```
BEGIN TRANSACTION;

UPDATE worker
SET active = @active
WHERE id = @id;

UPDATE job
SET state = @state,
next_run = DATETIME(DATETIME('now'), '+' || @next_run_in || ' seconds')
WHERE job.worker_id = @id;

INSERT INTO result(data, succeeded, acquired, elapsed, worker_id)
VALUES (@data, @succeeded, DATETIME('now'), @elapsed, @id);

COMMIT;
```

## Using gobal Ormo configuration

Ormo provides global [*OrmoConfiguration*](https://github.com/rexTexTau/Ormo/blob/main/Ormo/OrmoConfiguration.cs) singleton static class to make usage of queries and commands more streamlined. Calling methods of commands and queries also becomes more neat, cause in that case there is no need to pass field name converters, script providers and database connections as parameters – Ormo will simply use globals.

*Sample calling code wihout utilizing OrmoConfiguration*:
```
var result = await new SomeQuery(/*instance of the query script provider*/).Setup(/*query parameters*/).RunAsync(/*connection to the queries database*/);
var succeeded = await new SomeCommand(/*instance of the command script provider*/).Setup(/*command parameters*/).RunAsync(/*connection to the commands database*/);
```

*Sample calling code that utilizes OrmoConfiguration*:
```
// do it once at initialization time
OrmoConfiguration.Global.DefaultCommandScriptProvider = /*instance of the command script provider*/;
OrmoConfiguration.Global.DefaultQueryScriptProvider = /*instance of the query script provider*/;
OrmoConfiguration.Global.DefaultCommandConnection = /*connection to the commands database*/;
OrmoConfiguration.Global.DefaultQueryConnection = /*connection to the queries database*/;

// call commands and queries just like that
var result = await new SomeQuery().Setup(/*query parameters*/).RunAsync();
var succeeded = await new SomeCommand().Setup(/*command parameters*/).RunAsync();
```

Note: there is obviously no need to use separate databases (and database connections) for commands and queries if you do not want to. In case of single database used by both queries and commands, simply set equal DefaultCommandConnection and DefaultQueryConnection:
```
OrmoConfiguration.Global.DefaultCommandConnection = /*connection to the database*/;
OrmoConfiguration.Global.DefaultQueryConnection = /*connection to the database*/;
```

## Synchronous execution
Ormo queries and commands can be easily executed in synchronous codebase using *Run* method (instead of *RunAsync*):

```
var result = new SomeQuery(/*instance of the query script provider*/).Setup(/*query parameters*/).Run(/*connection to the queries database*/);
var succeeded = new SomeCommand(/*instance of the command script provider*/).Setup(/*command parameters*/).Run(/*connection to the commands database*/);
```

*Run* method accepts same parameters *RunAsync* does.

## Dapper comparison
Ormo is very similar to the well-known .Net library called [**Dapper**](https://github.com/DapperLib/Dapper).

Key differences are:
- Ormo is CQRS-styled by design, while Dapper is not;
- Ormo does handy and extensible SQL scripts management, while Dapper does not care where your SQL scripts come from at all;
- Ormo does no caching at all (but allows consumer to implement it in a relatively handy way), while Dapper uses internal [*CacheInfo*](https://stackoverflow.com/questions/9897750/what-exactly-is-the-information-that-dapper-caches) class;
- Ormo has ways to omit costly RTTI operations, while Dapper relies on RTTI (that means that Ormo can be faster in some cases);
- Ormo uses strongly-typed parameter classes, while Dapper uses stongly-typed as well as dynamic ones (PRs are welcome);
- Ormo does not support array/enumerable/list parameters and bulk operations at the moment, while Dapper does (PRs are welcome);
- Dapper has much more mature, stable and reliable codebase at all (PRs are welcome).

# Database migration
Ormo does not provide any database migration mechanics, use [DBUp](https://github.com/DbUp/DbUp) (which has a similar approach: it also uses SQL scripts embedded into an assembly), [Liquibase](https://www.liquibase.com/) or another migrator that fits your needs.

# .Net version
Ormo targets .NetStandard 2.1, because it heavily relies on C# 8.0 language features. A downgrade to older .Net Framework versions is quite possible, but the code will lose lots of conciseness and beauty in that case. Do it on your own risk.

# Legal Notice
Ormo is licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0) License. 

Full license text here: https://creativecommons.org/licenses/by-nc/4.0/legalcode

Human-readable summary here: https://creativecommons.org/licenses/by-nc/4.0/

You can obtain a less restrictive (dual) license on a paid basis. Get in touch:

- [@rextextau](https://t.me/rextextau)
- [rextextau@gmail.com](mailto:rextextau@gmail.com)
