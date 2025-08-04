-- Trigger for AFTER INSERT on Activity
CREATE TRIGGER trg_Activity_AfterInsert
AFTER INSERT ON Activity
FOR EACH ROW
BEGIN
    UPDATE Project
    SET Id = (
        SELECT
            CASE
                -- Condition 1: Project has NO Activity (excluding the newly inserted one IF it's the first)
                -- This subquery checks the count *after* the insert for NEW.Id
                WHEN NOT EXISTS (SELECT 1 FROM Activity act WHERE act.ProjectID = NEW.Id) THEN
                    -- If no Activity, revert to the project's manually set status (original intention)
                    (SELECT Id FROM Project WHERE ProjectID = NEW.Id)

                -- Condition 2: Project has 1 or more Activity - Apply hierarchy
                WHEN EXISTS (SELECT 1 FROM Activity act WHERE act.ProjectID = NEW.Id AND act.ActivityProgress = 5) THEN 5 -- Incomplete
                WHEN EXISTS (SELECT 1 FROM Activity act WHERE act.ProjectID = NEW.Id AND act.ActivityProgress = 6) THEN 6 -- On-Hold
                -- All Completed: Check if no Activity are *not* Completed (4)
                WHEN NOT EXISTS (SELECT 1 FROM Activity act WHERE act.ProjectID = NEW.Id AND act.ActivityProgress != 4) THEN 4 -- Completed
                WHEN EXISTS (SELECT 1 FROM Activity act WHERE act.ProjectID = NEW.Id AND act.ActivityProgress = 3) THEN 3 -- Ongoing
                WHEN EXISTS (SELECT 1 FROM Activity act WHERE act.ProjectID = NEW.Id AND act.ActivityProgress = 7) THEN 7 -- Sent for Review
                WHEN EXISTS (SELECT 1 FROM Activity act WHERE act.ProjectID = NEW.Id AND act.ActivityProgress = 2) THEN 2 -- Started
                -- All Upcoming: Check if no Activity are *not* Upcoming (1)
                WHEN NOT EXISTS (SELECT 1 FROM Activity act WHERE act.ProjectID = NEW.Id AND act.ActivityProgress != 1) THEN 1 -- Upcoming
                
                ELSE NULL -- Fallback
            END
    )
    WHERE ProjectID = NEW.Id;
END;

SELECT name FROM sqlite_master
WHERE type = 'trigger';
