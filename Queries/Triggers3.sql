-- SQLite
CREATE TRIGGER trg_status2
AFTER INSERT ON [Status]
BEGIN
    insert into Subdept(SubdeptName)
    values('hting');
END;

CREATE TRIGGER trg_Users
AFTER INSERT ON AspNetUsers
BEGIN
    INSERT INTO User (Id, UserName, Email, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled,LockoutEnabled, AccessFailedCount)
    VALUES (NEW.Id, NEW.UserName, new.Email, new.EmailConfirmed, new.PhoneNumberConfirmed, new.TwoFactorEnabled, new.LockoutEnabled, new.AccessFailedCount);
END;


SELECT name FROM sqlite_master
WHERE type = 'trigger';

DROP TRIGGER IF EXISTS trg_Users;