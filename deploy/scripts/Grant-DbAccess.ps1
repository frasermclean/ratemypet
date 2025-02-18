[CmdletBinding()]
param (
  [Parameter(Mandatory)][string] $PrincipalName,
  [Parameter(Mandatory)][string] $ConnectionString
)

Write-Host "Granting access to $PrincipalName"

$query = @"
IF NOT EXISTS (
  SELECT * FROM sys.database_principals
  WHERE [name] = '$PrincipalName'
)
BEGIN
  CREATE USER [$PrincipalName] FROM EXTERNAL PROVIDER;
  ALTER ROLE db_datareader ADD MEMBER [$PrincipalName];
  ALTER ROLE db_datawriter ADD MEMBER [$PrincipalName];
  ALTER ROLE db_ddladmin ADD MEMBER [$PrincipalName];
END
"@

Invoke-Sqlcmd -Query $query -ConnectionString $ConnectionString
