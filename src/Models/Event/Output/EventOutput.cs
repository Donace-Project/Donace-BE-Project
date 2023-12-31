﻿namespace Donace_BE_Project.Models.Event.Output;

public class EventOutput
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

    public Guid CalendarId { get; set; }

    public bool IsLive
    {
        get
        {
            return StartDate <= DateTime.Now && DateTime.Now <= EndDate;
        }
    }
    public string Status { get; set; } = string.Empty;

    public bool IsHost { get; set; } = true;
    public bool IsOnline { get; set; }
    public string LinkMeet { get; set; } = string.Empty;

    public int Sorted { get; set; }
}
