# SQL Connection Pooling (Bad Example)

This example demonstrates a **BAD** practice of leaving `SqlConnection` instances open. The result is a build-up of *sleeping* connections when `sp_who '<name>'` is ran. 

## Setup

Configure a connection to a database in `appsettings.json` under the `ConnectionStrings.Default` property. If using SQL Authentication, be sure to note the username/ID. 

**The command in this demo does not access nor modify data, but merely accesses the SQL server's current time.**


## Invocation

```powershell
# run the application
dotnet run

# make multiple requests, invoking the connection
curl.exe http://localhost:5292
```

The response of the request will return some metadata about the SQL connection itself as well as the results of multiple commands invoked on the same connection.

In SSMS, run the following to see how many connections have been established:

```sql
-- use the name of the connecting user
sp_who 'CONNECTION-USERNAME'
```

After each invocation of the request, it will create a new *sleeping* connection once it has completed the command. The value of `spid` in the `sp_who` command will correlate to the value of `serverProcessId` in the web response. 

In this example, if the `curl` command above is invoked 4 times, it will create 4 entries:

| spid | ecid | status | loginame |	hostname | blk | dbname | cmd | request_id |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 54 | 0 |sleeping | ryan | DESKTOP-VTKLL5U | 0 | Clients | AWAITING COMMAND | 0 |
| 61 | 0 |sleeping | ryan | DESKTOP-VTKLL5U | 0 | Clients | AWAITING COMMAND | 0 |
| 62 | 0 |sleeping | ryan | DESKTOP-VTKLL5U | 0 | Clients | AWAITING COMMAND | 0 |
| 63 | 0 |sleeping | ryan | DESKTOP-VTKLL5U | 0 | Clients | AWAITING COMMAND | 0 |

## Notes/Observations
- Each request to the endpoint creates a new connection from the pool, but because it is not closed, it is left in a *sleeping* state. 
- As soon as the web process terminates, the *sleeping* connections are removed.

