-- SQLite

DROP TRIGGER IF EXISTS trg_Users;

CREATE TRIGGER trg_Users
AFTER INSERT ON AspNetUsers
BEGIN
    INSERT INTO User (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount)
    VALUES (NEW.Id, NEW.UserName, NEW.NormalizedUserName, NEW.Email, NEW.NormalizedEmail, NEW.EmailConfirmed, NEW.PasswordHash, NEW.SecurityStamp, NEW.ConcurrencyStamp, NEW.PhoneNumber, NEW.PhoneNumberConfirmed, NEW.TwoFactorEnabled, NEW.LockoutEnd, NEW.LockoutEnabled, NEW.AccessFailedCount);
END;

CREATE TRIGGER trg_status
AFTER INSERT ON [Status]
BEGIN
    insert into Subdept(SubdeptName)
    values(NEW.SubdeptName)
END;

SELECT name FROM sqlite_master
WHERE type = 'trigger';