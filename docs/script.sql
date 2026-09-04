USE ST10479223;
GO

CREATE TABLE [User] (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Organiser', 'Participant')),
    FullName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Organiser (
    OrganiserID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE,
    OrganisationName NVARCHAR(100) NULL,
    BusinessPhone NVARCHAR(20) NULL,
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);

CREATE TABLE Participant (
    ParticipantID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE,
    DateOfBirth DATE NOT NULL,
    EmergencyContactName NVARCHAR(100) NULL,
    EmergencyContactPhone NVARCHAR(20) NULL,
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);

CREATE TABLE Event (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserID INT NOT NULL,
    Title NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500) NULL,
    EventDate DATE NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    MaxParticipants INT NULL,
    Status NVARCHAR(20) DEFAULT 'Upcoming' CHECK (Status IN ('Upcoming', 'Ongoing', 'Completed', 'Cancelled')),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (OrganiserID) REFERENCES Organiser(OrganiserID) ON DELETE CASCADE
);

CREATE TABLE Category (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    Distance DECIMAL(5,2) NOT NULL,
    EntryFee DECIMAL(10,2) NOT NULL,
    StartTime TIME NOT NULL,
    MaxParticipants INT NULL,
    FOREIGN KEY (EventID) REFERENCES Event(EventID) ON DELETE CASCADE
);

CREATE TABLE Enrolment (
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL,
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,
    RegistrationDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(20) DEFAULT 'Registered' CHECK (Status IN ('Registered', 'Confirmed', 'Withdrawn', 'Completed')),
    CONSTRAINT UQ_Enrolment_ParticipantEvent UNIQUE (ParticipantID, EventID),
    FOREIGN KEY (ParticipantID) REFERENCES Participant(ParticipantID),
    FOREIGN KEY (EventID) REFERENCES Event(EventID),
    FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
);

CREATE TABLE [Result] (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentID INT NOT NULL UNIQUE,
    FinishTime TIME NOT NULL,
    OverallPosition INT NULL,
    CategoryPosition INT NULL,
    IsDisqualified BIT DEFAULT 0,
    Notes NVARCHAR(255) NULL,
    FOREIGN KEY (EnrolmentID) REFERENCES Enrolment(EnrolmentID) ON DELETE CASCADE
);
GO

INSERT INTO [User] (Email, PasswordHash, Role, FullName, PhoneNumber) VALUES
('thabo@runsa.co.za', 'HASH_1', 'Organiser', 'Thabo Mokoena', '0821112233'),
('linda@cyclect.co.za', 'HASH_2', 'Organiser', 'Linda Van Wyk', '0834445566'),
('siya.runner@gmail.com', 'HASH_3', 'Participant', 'Siya Khumalo', '0715556677'),
('chloe.walker@outlook.com', 'HASH_4', 'Participant', 'Chloe Walker', '0728889900');

INSERT INTO Organiser (UserID, OrganisationName, BusinessPhone) VALUES
(1, 'Run South Africa Events', '011 345 6789'),
(2, 'Cape Town Cycle Promotions', '021 987 6543');

INSERT INTO Participant (UserID, DateOfBirth, EmergencyContactName, EmergencyContactPhone) VALUES
(3, '1995-03-15', 'Zanele Khumalo', '0715556678'),
(4, '1988-11-22', 'Dave Walker', '0728889901');

INSERT INTO Event (OrganiserID, Title, Description, EventDate, Location, MaxParticipants) VALUES
(1, 'Soweto Marathon 2026', 'Iconic 42.2km road race through Soweto.', '2026-11-02', 'FNB Stadium, Soweto', 5000),
(1, 'Johannesburg 10k Night Run', 'A fast 10km night race.', '2026-12-05', 'Sandton CBD', 2000),
(2, 'Cape Town Cycle Tour 2026', 'The world''s largest timed cycle race.', '2026-03-08', 'Cape Town Stadium', 10000);

INSERT INTO Category (EventID, Name, Distance, EntryFee, StartTime, MaxParticipants) VALUES
(1, 'Full Marathon', 42.2, 350.00, '05:30:00', 3000),
(1, 'Half Marathon', 21.1, 250.00, '06:30:00', 2000),
(2, '10km Road', 10.0, 150.00, '19:00:00', 2000),
(3, 'Standard Cycle', 109.0, 500.00, '06:00:00', 9000),
(3, 'Elite Cycle', 109.0, 750.00, '05:45:00', 1000);

INSERT INTO Enrolment (ParticipantID, EventID, CategoryID, Status) VALUES
(1, 1, 1, 'Confirmed'),
(1, 2, 3, 'Registered'),
(2, 3, 4, 'Confirmed');

INSERT INTO [Result] (EnrolmentID, FinishTime, OverallPosition, CategoryPosition, IsDisqualified, Notes) VALUES
(1, '04:15:32', 125, 45, 0, NULL),
(3, '03:58:12', 320, 80, 0, NULL);
GO

SELECT 'Users' as TableName, * FROM [User];
SELECT 'Organisers' as TableName, * FROM Organiser;
SELECT 'Participants' as TableName, * FROM Participant;
SELECT 'Events' as TableName, * FROM Event;
SELECT 'Categories' as TableName, * FROM Category;
SELECT 'Enrolments' as TableName, * FROM Enrolment;
SELECT 'Results' as TableName, * FROM [Result];