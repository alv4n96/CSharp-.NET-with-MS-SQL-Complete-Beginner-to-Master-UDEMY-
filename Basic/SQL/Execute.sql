USE DotNetCourseDB;
GO

SELECT
    [Users].[UserId],
    [Users].[FirstName] + ' ' + [Users].[LastName] AS FullName,
    -- [Users].[Email],
    -- [Users].[Gender],
    -- [Users].[Active],
    [UserJobInfo].[JobTitle],
    [UserSalary].[Salary]
-- COUNT(*) AS UsersCount
FROM TutorialAppSchema.Users AS Users
    JOIN TutorialAppSchema.UserSalary AS UserSalary
    ON Users.UserId = UserSalary.UserId
    LEFT JOIN TutorialAppSchema.UserJobInfo AS UserJobInfo
    ON Users.UserId = UserJobInfo.UserId
WHERE Users.Active = 1
ORDER BY UserSalary.Salary DESC;

CREATE CLUSTERED INDEX cix_UserSalary_UserId ON TutorialAppSchema.UserSalary(UserId);

CREATE NONCLUSTERED INDEX ix_UserJobInfo_JobTitle ON TutorialAppSchema.UserJobInfo(JobTitle) INCLUDE (Department);

CREATE NONCLUSTERED INDEX fix_Users_Active ON TutorialAppSchema.Users(Active) 
    INCLUDE ([Email], [FirstName], [LastName])
        WHERE Active=1;

-- DROP INDEX ix_Users_JobTitle ON TutorialAppSchema.Users;

SELECT 
    ISNULL([UserJobInfo].[Department],'Not Departement Listed') AS Department
    -- ,[UserJobInfo].[Department]
    ,SUM([UserSalary].[Salary]) AS TotalSalary
    ,MIN([UserSalary].[Salary]) AS MinSalary
    ,MAX([UserSalary].[Salary]) AS MaxSalary
    ,AVG([UserSalary].[Salary]) AS AvgSalary
    -- ,COUNT([UserJobInfo].[Department]) AS TotalEmployeeEachDept
    ,COUNT(*) AS TotalEmployeeEachDept
    -- ,STRING_AGG(Users.UserId, ', ') AS UserIds
FROM TutorialAppSchema.Users AS Users
    JOIN TutorialAppSchema.UserSalary AS UserSalary
    ON Users.UserId = UserSalary.UserId
    LEFT JOIN TutorialAppSchema.UserJobInfo AS UserJobInfo
    ON UserJobInfo.UserId = Users.UserId
WHERE Users.Active = 1
GROUP BY [UserJobInfo].[Department]
ORDER BY [UserJobInfo].[Department] DESC;



SELECT
    Users.UserId,
    Users.FirstName + ' ' + Users.LastName AS FullName,
    UserJobInfo.JobTitle,
    COUNT(*) AS JobCount,
    COUNT(*) OVER() AS TotalRowCount
FROM TutorialAppSchema.Users AS Users
    JOIN TutorialAppSchema.UserJobInfo AS UserJobInfo
    ON Users.UserId = UserJobInfo.UserId
WHERE Users.Active = 1
GROUP BY 
    Users.UserId,
    Users.FirstName,
    Users.LastName,
    UserJobInfo.JobTitle
ORDER BY Users.UserId DESC;


