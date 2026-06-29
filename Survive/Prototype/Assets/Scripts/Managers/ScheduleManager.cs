using System;
using System.Collections.Generic;
using UnityEngine;

public class ScheduleManager  // schedule manager generates data based on hard codded values 
{
    public static ScheduleManager Instance;
    private Dictionary<Species, List<Schedule>> _scheduleTemplate;
    private List<string> listofnames = new();
    public void GenerateSchedule()
    {

    _scheduleTemplate = new Dictionary<Species, List<Schedule>>();
        
    _scheduleTemplate[Species.Deer] = GenerateRandomSchedule(Species.Deer);
   // _scheduleTemplate[Species.Antelope] = GenerateRandomSchedule(Species.Antelope);
    _scheduleTemplate[Species.Horse] = GenerateRandomSchedule(Species.Horse);
    
foreach (var kvp in _scheduleTemplate)
{
    Species species = kvp.Key;
    List<Schedule> schedules = kvp.Value;

    foreach (var entry in schedules)
    {
//        Debug.Log($"{species}: {entry.zoneType} {entry.startHour}-{entry.endHour}");
    }
}

    }
    private List<Schedule> GenerateRandomSchedule(Species species)// create blocks at 5 Am 
    {
        // Activities available for random selection
        List<Activity> activities = new List<Activity>
        {
            Activity.Feeding,
            Activity.Drinking,
            Activity.Resting
        };

        // Decide how many blocks the day will have (2–4)
        int blockCount = UnityEngine.Random.Range(3, 8);

        // Ensure resting is always included
        List<Activity> chosen = new List<Activity>();
        chosen.Add(Activity.Resting);

        // Fill remaining blocks with random activities
        for (int i = 1; i < blockCount; i++)
        {
            Activity a = activities[UnityEngine.Random.Range(0, activities.Count)];
            chosen.Add(a);
        }

        List<Schedule> schedule = new List<Schedule>();
        schedule.Add(new Schedule
        {
            species = species,
            zoneType = chosen[0],
            startHour = 0,
            endHour = 5
        });

        // Remaining hours = 19
        int remainingHours = 19;
        int remainingBlocks = blockCount - 1;

        // Generate durations for remaining blocks
        List<int> durations = RandomDurations(remainingBlocks, remainingHours);

        int currentHour = 5;

        for (int i = 1; i < blockCount; i++)
        {
            int start = currentHour;
            int end = currentHour + durations[i - 1];

            if (i == blockCount - 1)
                end %= 24;

            schedule.Add(new Schedule
            {
                species = species,
                zoneType = chosen[i],
                startHour = start,
                endHour = end
            });

            currentHour = end;
        }

        return schedule;
    }


    private List<int> RandomDurations(int count, int total)
    {
        List<int> cuts = new List<int>();

        // random cut points
        for (int i = 0; i < count - 1; i++)
            cuts.Add(UnityEngine.Random.Range(1, total));

        cuts.Sort();

        List<int> durations = new List<int>();
        int prev = 0;

        foreach (int c in cuts)
        {
            durations.Add(c - prev);
            prev = c;
        }
        durations.Add(total - prev);
        return durations;
    }

    public List<Schedule> GetSchedule(Species species, Bounds regionBounds)// this function assigns zone to the schedule if there are no a
    {
        if(!_scheduleTemplate.ContainsKey(species))
        {  return null;}

        var schedule = _scheduleTemplate[species];
        foreach (var entry in schedule)
        {
            entry.assignedZone = ZoneManager.Instance.GetAvailableZone(
                entry.zoneType,
                entry.species,
                entry.startHour,
                entry.endHour,
                regionBounds);
        }
        return schedule;
    }
}
