-- SQLite
INSERT INTO User (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount)

SELECT Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount
FROM AspNetUsers
where Id not in 
(select Id from User)

CREATE TRIGGER trg_Users
AFTER INSERT ON Products
BEGIN
    INSERT INTO ProductAuditLog (product_id, action, new_price, new_stock)
    VALUES (NEW.product_id, 'INSERT', NEW.price, NEW.stock);
END;