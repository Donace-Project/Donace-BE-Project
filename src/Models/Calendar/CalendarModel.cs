using Donace_BE_Project.Models.Cache;
using System.ComponentModel.DataAnnotations;

namespace Donace_BE_Project.Models.Calendar;

public class CalendarModel
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string Cover { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public string PublicURL { get; set; } = string.Empty;

    public string Lat { get; set; } = string.Empty;

    public string Long { get; set; } = string.Empty;

    public string AddressName { get; set; } = string.Empty;
}

public class CalendarUpdateModel
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string Cover { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public string PublicURL { get; set; } = string.Empty;

    public string Lat { get; set; } = string.Empty;

    public string Long { get; set; } = string.Empty;

    public string AddressName { get; set; } = string.Empty;
}

public class GetListCalendarModel : CacheSortedBaseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public int TotalSubcriber { get; set; }
    public string Avatar { get; set; }
    public Guid UserId { get; set; }
    public bool IsSubcribed { get; set; } = false;
}

public class GetListUserInCalendarModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreationTime { get; set; }
}

public class RequestGetListUserInCalendarModel : RequestBaseModel
{
    public Guid Id { get; set;}
    public string Keyword { get; set; } = string.Empty;

    public bool IsSubcribed { get; set;}
}

public class CalendarResponseModel : CacheSortedBaseModel
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string Cover { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public string PublicURL { get; set; } = string.Empty;

    public string Lat { get; set; } = string.Empty;

    public string Long { get; set; } = string.Empty;

    public string AddressName { get; set; } = string.Empty;

    public int TotalSubcribed { get; set;}

    public bool IsHost { get; set; } = false;

    public bool IsSub { get; set; } = false;
}

public class InviteJoinCalendarModel
{
    public string Email { get; set; }

    public string UrlInfo { get; set; }

    public string CalendarName { get; set; } = string.Empty;
}