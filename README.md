Prepair for start :
Open file appsetting.json and change "ConnectionString" for your PostgreSQL connection string

For start project select EventMaster/Server/EventMaster or use the command in cmd : 
cd EventMaster/Server/EventMaster
after that use thoose commands :
dotnet restore
dotnet build
dotnet run --project EventMaster.Presentation
