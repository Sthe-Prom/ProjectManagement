-- SQLite
INSERT INTO Status (Id, StatusName)
VALUES ();

CREATE TRIGGER trg_status
AFTER INSERT ON Status
BEGIN
    insert into Subdept(SubdeptName)
    values(new.StatusName);
END;

CREATE TRIGGER trg_Users
AFTER INSERT ON AspNetUsers
BEGIN
    insert into User(Id, UserName, Email, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
    VALUES (new.Id, new.UserName, new.Email, new.EmailConfirmed, new.PhoneNumberConfirmed, new.TwoFactorEnabled, new.LockoutEnabled, new.AccessFailedCount);
END;

SELECT name FROM sqlite_master
WHERE type = 'trigger';


DROP TRIGGER IF EXISTS trg_Users;
