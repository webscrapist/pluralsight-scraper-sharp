-- courses in channel

select C."Name" ChannelName
     ,C2."Name" CourseName
     ,C2."Level"
     ,C2."DatePublished"
     --, C2.*
from "Channels" C
join "ChannelCourse" CC on C."Id" = CC."ChannelId"
join "Courses" C2 on CC."CourseId" = C2."Id"
where C."Name" = 'PowerShell';