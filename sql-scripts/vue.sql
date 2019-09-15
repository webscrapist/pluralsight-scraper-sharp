select ch."Name" "channel", cr."Name" "course", cr."Level" "level", cr."DatePublished" "publish date"
from public."Channels" ch
left join public."ChannelCourse" cc on cc."ChannelId" = ch."Id"
left join public."Courses" cr on cr."Id" = cc."CourseId"
where ch."Name" in ('WebDev/CSS', 'WebDev/Frameworks/Vue', 'WebDev/Tools')
order by ch."Name", cr."Name";
