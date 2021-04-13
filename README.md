# SqlClientEFCore

Nuget (C# library) EntityFrameworkCore SQL Client for Exec StoredProcedure and Command text.


## Console App Example

```csharp
    public class Test : DbContext
    {
        public Test() : base(SQLBuilder.GetOptions(EncryptionMethods.Instance.AES_Decrypt("encrypted_conn_str"))) //or : base(SQLBuilder.GetOptions(dBConnections.GetConnStr("conn_str")))
        {
        }

        public CheckRepository TestMethod()
        {
            var obj = this.LoadStoredProcedure("SP_CheckRepository")
                .AddParam("@Date", DateTime.Now)
                .ExecuteSqlQuery<CheckRepository>();

            return obj.FirstOrDefault();
        }
    }
```

## API Example

```csharp
    public class TestRepository : DbContext, ITestRepository
    {
        private readonly ILogger _logger;
        private IConfiguration _configuration;
        private readonly IDBConnections _dBConnections;
        private readonly ICoreDBContext _coreDBContext;

        public DatabaseFacade Database
        {
            get
            {
                return _coreDBContext.Database;
            }
        }
        public TestRepository(ICoreDBContext coreDBContext, ILogger logger, IConfiguration configuration, IDBConnections dBConnections) : base(SQLBuilder.GetOptions(dBConnections.GetConnStr("conn_str")))
        {
            _logger = logger;
            _configuration = configuration;
            _dBConnections = dBConnections;
            _coreDBContext = coreDBContext;
        }
		
            var obj = this.LoadStoredProcedure("SP_CheckRepository")
                .AddParam("@Date", DateTime.Now)
                .ExecuteSqlQuery<CheckRepository>();
		
```




## Installation

` Install-Package SqlClientEFCore `
