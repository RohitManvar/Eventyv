-- 1. Users Table
CREATE TABLE Users (
    Id NVARCHAR(450) PRIMARY KEY, -- Identity PK is string (GUID/Identity UserId)
    UserName NVARCHAR(256) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    PhoneNumber NVARCHAR(20),
    FullName NVARCHAR(200),
    ProfilePictureUrl NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 2. Events Table
CREATE TABLE Events (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    EventDate DATETIME NOT NULL,
    Location NVARCHAR(300),
    Speaker NVARCHAR(200),
    Agenda NVARCHAR(MAX),
    MaxAttendees INT NULL,
    OrganizerId NVARCHAR(450) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_Events_Users FOREIGN KEY (OrganizerId) 
        REFERENCES Users(Id) ON DELETE CASCADE
);

-- 3. Registrations Table
CREATE TABLE Registrations (
    RegistrationId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL,
    UserId NVARCHAR(450) NOT NULL,
    RegisteredAt DATETIME DEFAULT GETDATE(),
    TicketCode NVARCHAR(50) UNIQUE NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_Registrations_Events FOREIGN KEY (EventId) 
        REFERENCES Events(EventId) ON DELETE CASCADE,
    CONSTRAINT FK_Registrations_Users FOREIGN KEY (UserId) 
        REFERENCES Users(Id) ON DELETE NO ACTION
);

-- 4. Tickets Table
CREATE TABLE Tickets (
    TicketId INT IDENTITY(1,1) PRIMARY KEY,
    RegistrationId INT NOT NULL UNIQUE,
    TicketUrl NVARCHAR(500),
    GeneratedAt DATETIME,
    CONSTRAINT FK_Tickets_Registrations FOREIGN KEY (RegistrationId) 
        REFERENCES Registrations(RegistrationId) ON DELETE CASCADE
);

-- 5. EmailNotifications Table
CREATE TABLE EmailNotifications (
    NotificationId INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL,
    EventId INT NOT NULL,
    Subject NVARCHAR(200) NOT NULL,
    Body NVARCHAR(MAX),
    SentAt DATETIME,
    Status NVARCHAR(50),
    CONSTRAINT FK_EmailNotifications_Users FOREIGN KEY (UserId) 
        REFERENCES Users(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_EmailNotifications_Events FOREIGN KEY (EventId) 
        REFERENCES Events(EventId) ON DELETE CASCADE
);

-- 6. PaymentMethods Table (lookup)
CREATE TABLE PaymentMethods (
    PaymentMethodId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(300),
    IsActive BIT DEFAULT 1
);

-- 7. Payments Table (per registration)
CREATE TABLE Payments (
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    RegistrationId INT NOT NULL,
    PaymentMethodId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Currency NVARCHAR(10) NOT NULL,
    TransactionId NVARCHAR(100),
    Status NVARCHAR(50) NOT NULL,
    PaidAt DATETIME NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Payments_Registrations FOREIGN KEY (RegistrationId) 
        REFERENCES Registrations(RegistrationId) ON DELETE CASCADE,
    CONSTRAINT FK_Payments_PaymentMethods FOREIGN KEY (PaymentMethodId) 
        REFERENCES PaymentMethods(PaymentMethodId)
);
