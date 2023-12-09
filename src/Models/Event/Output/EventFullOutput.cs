﻿using Donace_BE_Project.Enums.Entity;
using Donace_BE_Project.Models.Cache;

namespace Donace_BE_Project.Models.Event.Output;

public class EventFullOutput : CacheSortedBaseModel
{
    public Guid Id { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string AddressName { get; set; } = string.Empty;

    public string Lat { get; set; } = string.Empty;

    public string Long { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public bool IsOverCapacity { get; set; }

    public string Cover { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Theme { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public float FontSize { get; set; }

    public string Instructions { get; set; } = string.Empty;

    public bool IsMultiSection { get; set; }

    public float Duration { get; set; }

    public int TotalGuest { get; set; }

    public ICollection<SectionOutput> Sections { get; set; } = default!;

    public Guid CalendarId { get; set; }

    public bool IsHost { get; set; } = true;

    public string Status { get; set; } = string.Empty;

    public bool IsOnline { get; set; }
    public string LinkMeet { get; set; } = string.Empty;
}

public class EventsModelResponse
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
}

public class EventDetailModel : EventFullOutput
{
    public bool IsSub { get; set; } = true;

    public bool IsAppro { get; set; } = true;
}