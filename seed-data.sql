-- =============================================
-- TaskFlow Seed Data Script
-- Run this in SQL Server Management Studio 21
-- Database: TaskFlowDb
-- =============================================

-- Use the TaskFlow database
-- (Create it first if it doesn't exist)
-- CREATE DATABASE TaskFlowDb;
-- GO
-- USE TaskFlowDb;
-- GO

-- =============================================
-- 1. User Roles
-- =============================================
INSERT INTO [UserRole] ([guid], [RoleName], [RoleNameAr], [IsDeleted], [IsActive], [CreationDate])
VALUES
    (NEWID(), 'Admin', N'مدير', 0, 1, GETDATE()),
    (NEWID(), 'User', N'مستخدم', 0, 1, GETDATE());

-- =============================================
-- 2. Statuses
-- =============================================
INSERT INTO [Status] ([guid], [StatusName], [StatusNameAr], [IsDeleted], [IsActive], [CreationDate])
VALUES
    (NEWID(), 'Pending', N'قيد الانتظار', 0, 1, GETDATE()),
    (NEWID(), 'In Progress', N'قيد التنفيذ', 0, 1, GETDATE()),
    (NEWID(), 'Completed', N'مكتمل', 0, 1, GETDATE()),
    (NEWID(), 'Cancelled', N'ملغي', 0, 1, GETDATE()),
    (NEWID(), 'On Hold', N'معلق', 0, 1, GETDATE());

-- =============================================
-- 3. System Users
-- =============================================
-- Password hashes (PBKDF2/SHA256, 100k iterations):
-- Admin@123 -> oxphcVgUP0vIc8vYBUTn1MnBMzqh5Q+Fc6QjGfg/urbbHmYv1AaTUdQz6Vwi9gjq
-- Password123 -> EongQFcOYKVfNTXkubloSPS9HizcmJaJoRsQ6b1ihA+hmEcjukjY3/pwZn4HKq3F
-- User@123 -> 1WXraP14tMjyza6ipf1dWtQyrQim+7gOkDt4lGszU+nXPNHl+7ZkcNdTb9ZgOM/q
-- Test@123 -> NnlRcgFHU1UQjuIJsNn9iPAieEbA6HsNDFH7sKYCd70R7POzR4+yn2BSlsb0M9yI

INSERT INTO [SystemUser] ([guid], [Email], [Name], [NameAr], [Password], [UserRoleId], [IsDeleted], [IsActive], [CreationDate])
VALUES
    -- Admin account (RoleId 1)
    (NEWID(), 'admin@taskflow.com', 'Ahmed Al-Qahtani', N'أحمد القحطاني',
     'oxphcVgUP0vIc8vYBUTn1MnBMzqh5Q+Fc6QjGfg/urbbHmYv1AaTUdQz6Vwi9gjq',
     1, 0, 1, GETDATE()),

    -- User accounts (RoleId 2)
    (NEWID(), 'sarah@taskflow.com', 'Sarah Al-Mutairi', N'سارة المطيري',
     'EongQFcOYKVfNTXkubloSPS9HizcmJaJoRsQ6b1ihA+hmEcjukjY3/pwZn4HKq3F',
     2, 0, 1, GETDATE()),

    (NEWID(), 'fahad@taskflow.com', 'Fahad Al-Otaibi', N'فهد العتيبي',
     '1WXraP14tMjyza6ipf1dWtQyrQim+7gOkDt4lGszU+nXPNHl+7ZkcNdTb9ZgOM/q',
     2, 0, 1, GETDATE()),

    (NEWID(), 'nora@taskflow.com', 'Nora Al-Shehri', N'نورة الشهري',
     'EongQFcOYKVfNTXkubloSPS9HizcmJaJoRsQ6b1ihA+hmEcjukjY3/pwZn4HKq3F',
     2, 0, 1, GETDATE()),

    (NEWID(), 'mohammed@taskflow.com', 'Mohammed Al-Dossari', N'محمد الدوسري',
     'NnlRcgFHU1UQjuIJsNn9iPAieEbA6HsNDFH7sKYCd70R7POzR4+yn2BSlsb0M9yI',
     2, 0, 1, GETDATE());

-- =============================================
-- 4. Projects (UserId 2 = Sarah, 3 = Fahad, 4 = Nora, 5 = Mohammed)
-- =============================================
DECLARE @Project1 UNIQUEIDENTIFIER = NEWID();
DECLARE @Project2 UNIQUEIDENTIFIER = NEWID();
DECLARE @Project3 UNIQUEIDENTIFIER = NEWID();
DECLARE @Project4 UNIQUEIDENTIFIER = NEWID();
DECLARE @Project5 UNIQUEIDENTIFIER = NEWID();

INSERT INTO [Project] ([guid], [Name], [Description], [Color], [UserId], [IsDeleted], [IsActive], [CreationDate])
VALUES
    (@Project1, N'Company Website Redesign', N'Complete redesign of the corporate website with modern UI/UX', '#0f7179', 2, 0, 1, DATEADD(day, -30, GETDATE())),
    (@Project2, N'Mobile App Development', N'Build a cross-platform mobile application using .NET MAUI', '#6d28d9', 3, 0, 1, DATEADD(day, -21, GETDATE())),
    (@Project3, N'E-commerce Platform', N'Develop an online store with payment gateway integration', '#dc2626', 2, 0, 1, DATEADD(day, -14, GETDATE())),
    (@Project4, N'Data Analytics Dashboard', N'Create interactive dashboards for business intelligence', '#0891b2', 4, 0, 1, DATEADD(day, -7, GETDATE())),
    (@Project5, N'Internal HR System', N'Build an employee management system for HR department', '#059669', 5, 0, 1, DATEADD(day, -3, GETDATE()));

-- =============================================
-- 5. Categories
-- =============================================
DECLARE @Cat1 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cat2 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cat3 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cat4 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cat5 UNIQUEIDENTIFIER = NEWID();

INSERT INTO [Category] ([guid], [Name], [Color], [UserId], [IsDeleted], [IsActive], [CreationDate])
VALUES
    (@Cat1, N'Design', '#0f7179', 2, 0, 1, GETDATE()),
    (@Cat2, N'Development', '#6d28d9', 2, 0, 1, GETDATE()),
    (@Cat3, N'Testing', '#dc2626', 3, 0, 1, GETDATE()),
    (@Cat4, N'Documentation', '#0891b2', 3, 0, 1, GETDATE()),
    (@Cat5, N'Meeting', '#059669', 4, 0, 1, GETDATE());

-- =============================================
-- 6. Todo Items
-- =============================================
-- Sarah's tasks (UserId = 2)
INSERT INTO [TodoItem] ([guid], [Title], [Description], [DueDate], [IsCompleted], [Priority], [ProjectId], [CategoryId], [UserId], [StatusId], [IsDeleted], [IsActive], [CreationDate])
VALUES
    (NEWID(), N'Design homepage wireframes',
     N'Create wireframes for the new company website homepage with hero section, features, and CTA',
     DATEADD(day, 5, GETDATE()), 0, 1, @Project1, @Cat1, 2, 2, 0, 1, DATEADD(day, -2, GETDATE())),

    (NEWID(), N'Implement user authentication',
     N'Add login/register functionality with JWT tokens, password hashing, and email verification',
     DATEADD(day, 10, GETDATE()), 0, 1, @Project3, @Cat2, 2, 1, 0, 1, DATEADD(day, -1, GETDATE())),

    (NEWID(), N'Create product catalog API',
     N'Build REST API endpoints for product listing, search, filtering and pagination',
     DATEADD(day, 7, GETDATE()), 0, 2, @Project3, @Cat2, 2, 2, 0, 1, DATEADD(day, -3, GETDATE())),

    (NEWID(), N'Review competitor websites',
     N'Analyze top 5 competitor websites and document UX patterns and design trends',
     DATEADD(day, -2, GETDATE()), 1, 3, @Project1, @Cat4, 2, 3, 0, 1, DATEADD(day, -10, GETDATE())),

    (NEWID(), N'Set up CI/CD pipeline',
     N'Configure GitHub Actions for automated build, test, and deployment to Azure',
     DATEADD(day, 3, GETDATE()), 0, 2, @Project1, @Cat2, 2, 1, 0, 1, DATEADD(day, -5, GETDATE())),

    (NEWID(), N'Sprint planning meeting',
     N'Prepare agenda and facilitate sprint planning for the upcoming development cycle',
     DATEADD(day, 1, GETDATE()), 0, 3, NULL, @Cat5, 2, 1, 0, 1, DATEADD(day, -1, GETDATE()));

-- Fahad's tasks (UserId = 3)
INSERT INTO [TodoItem] ([guid], [Title], [Description], [DueDate], [IsCompleted], [Priority], [ProjectId], [CategoryId], [UserId], [StatusId], [IsDeleted], [IsActive], [CreationDate])
VALUES
    (NEWID(), N'Design database schema',
     N'Create ERD and define tables, relationships, and indexes for the mobile app backend',
     DATEADD(day, 4, GETDATE()), 0, 1, @Project2, @Cat4, 3, 2, 0, 1, DATEADD(day, -4, GETDATE())),

    (NEWID(), N'Implement push notifications',
     N'Integrate Firebase Cloud Messaging for real-time push notifications across platforms',
     DATEADD(day, 12, GETDATE()), 0, 2, @Project2, @Cat2, 3, 1, 0, 1, DATEADD(day, -2, GETDATE())),

    (NEWID(), N'Write unit tests for API',
     N'Cover all API endpoints with xUnit tests, including edge cases and error scenarios',
     DATEADD(day, 8, GETDATE()), 0, 2, @Project2, @Cat3, 3, 1, 0, 1, DATEADD(day, -6, GETDATE())),

    (NEWID(), N'Create app icon and splash screen',
     N'Design app icon variants for iOS and Android, plus animated splash screen',
     DATEADD(day, -5, GETDATE()), 1, 3, @Project2, @Cat1, 3, 3, 0, 1, DATEADD(day, -14, GETDATE())),

    (NEWID(), N'API documentation',
     N'Document all REST endpoints using Swagger/OpenAPI with request/response examples',
     DATEADD(day, 6, GETDATE()), 0, 3, @Project2, @Cat4, 3, 1, 0, 1, DATEADD(day, -3, GETDATE()));

-- Nora's tasks (UserId = 4)
INSERT INTO [TodoItem] ([guid], [Title], [Description], [DueDate], [IsCompleted], [Priority], [ProjectId], [CategoryId], [UserId], [StatusId], [IsDeleted], [IsActive], [CreationDate])
VALUES
    (NEWID(), N'Design dashboard mockups',
     N'Create Figma mockups for the analytics dashboard with charts, filters, and data tables',
     DATEADD(day, 3, GETDATE()), 0, 1, @Project4, @Cat1, 4, 2, 0, 1, DATEADD(day, -1, GETDATE())),

    (NEWID(), N'Implement data visualization',
     N'Integrate Chart.js for interactive charts - bar, line, pie, and scatter plots',
     DATEADD(day, 14, GETDATE()), 0, 2, @Project4, @Cat2, 4, 1, 0, 1, DATEADD(day, -5, GETDATE())),

    (NEWID(), N'Set up ETL pipeline',
     N'Create data extraction and transformation jobs to feed the analytics database',
     DATEADD(day, 10, GETDATE()), 0, 1, @Project4, @Cat2, 4, 1, 0, 1, DATEADD(day, -3, GETDATE())),

    (NEWID(), N'Client presentation',
     N'Prepare and deliver the quarterly analytics review presentation to stakeholders',
     DATEADD(day, -3, GETDATE()), 1, 1, @Project4, @Cat5, 4, 3, 0, 1, DATEADD(day, -20, GETDATE()));

-- Mohammed's tasks (UserId = 5)
INSERT INTO [TodoItem] ([guid], [Title], [Description], [DueDate], [IsCompleted], [Priority], [ProjectId], [CategoryId], [UserId], [StatusId], [IsDeleted], [IsActive], [CreationDate])
VALUES
    (NEWID(), N'Design employee database schema',
     N'Create database schema for employees, departments, attendance, and payroll',
     DATEADD(day, 2, GETDATE()), 0, 1, @Project5, @Cat4, 5, 2, 0, 1, DATEADD(day, -1, GETDATE())),

    (NEWID(), N'Build leave management module',
     N'Implement leave request workflow with approval hierarchy and calendar integration',
     DATEADD(day, 20, GETDATE()), 0, 1, @Project5, @Cat2, 5, 1, 0, 1, DATEADD(day, -2, GETDATE())),

    (NEWID(), N'Test payroll calculation',
     N'Run comprehensive tests on payroll calculation including overtime, deductions, and bonuses',
     DATEADD(day, 5, GETDATE()), 0, 2, @Project5, @Cat3, 5, 1, 0, 1, DATEADD(day, -4, GETDATE())),

    (NEWID(), N'Deploy to staging server',
     N'Deploy the current build to staging environment for UAT testing',
     DATEADD(day, -1, GETDATE()), 1, 1, @Project5, @Cat2, 5, 3, 0, 1, DATEADD(day, -8, GETDATE()));

-- =============================================
-- Verify the data
-- =============================================
-- SELECT * FROM [UserRole];
-- SELECT * FROM [Status];
-- SELECT * FROM [SystemUser];
-- SELECT * FROM [Project];
-- SELECT * FROM [Category];
-- SELECT * FROM [TodoItem];
