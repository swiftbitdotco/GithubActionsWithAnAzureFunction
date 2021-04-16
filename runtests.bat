REM dotnet test --blame --no-restore --logger="trx;LogFileName=TestResults.trx" --results-directory="TestResults" /p:CollectCoverage=true /p:CoverletOutput="TestResults" /p:CoverletOutputFormat="cobertura"

REM dotnet test --blame --no-restore --logger="trx;LogFileName=TestResults.trx" --results-directory="TestResults" --collect="XPlat Code Coverage"
 
dotnet test --blame --no-restore --logger="trx;LogFileName=TestResults.trx" --results-directory="ASDF" --collect="XPlat Code Coverage"
