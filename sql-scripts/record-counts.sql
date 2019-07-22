set search_path = "public";

-- course count per channel

select C."Name", count(CC."CourseId") Courses
from "Channels" C
left join "ChannelCourse" CC on C."Id" = CC."ChannelId"
group by C."Name"
order by lower(C."Name")
;

-- channel count per course

select C."Name", count(CC."ChannelId") Channels
from "Courses" C
left join "ChannelCourse" CC on C."Id" = CC."CourseId"
group by C."Name"
order by lower(C."Name")
;
