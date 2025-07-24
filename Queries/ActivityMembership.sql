-- SQLite
select p.Id 'Project', a.Id 'Activity', c.FirstName, a.MemberProject
from Project p
 join Activity a
on p.Id == a.ProjectID
 join Account c
on p.AccountID == c.AccountID
where p.AccountID == 3 and p.Id in (8) --not in (p.SelectedAssignedUserIds)

select *
from Project p
--join Activity a
--on p.Id = a.ProjectID
Where p.AccountID = 3

select *
from Activity a
where a.MemberProject = 8

update Activity
set ProjectId = 8
where Id = 13