IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'TaskManagement')
BEGIN
    CREATE DATABASE [TaskManagement];
END
