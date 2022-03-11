using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

public class DayNightCycle : NetworkBehaviour
{
    private enum TimeOfDay { Morning, Noon, Evening, Night };

    [SerializeField] private float cycleInMinutes;
    private TimeOfDay timeOfDay;
    public NetworkVariable<float> decimalTime;

    public UnityEvent onMorning;
    public UnityEvent onNoon;
    public UnityEvent onEvening;
    public UnityEvent onNight;
    
    private void Update()
    {
        if (!IsServer)
        {
            Debug.Log(decimalTime.Value);
            return;
        };
        
        decimalTime.Value += (0.25f + Time.time * 6 / cycleInMinutes / 360) % 1;

        var lastTimeOfDay = timeOfDay;

        if (decimalTime.Value > 0.25f && decimalTime.Value < 0.5f)
        {
            timeOfDay = TimeOfDay.Morning;
        } 
        else if (decimalTime.Value > 0.5f && decimalTime.Value < 0.75f)
        {
            timeOfDay = TimeOfDay.Noon;
        }
        else if (decimalTime.Value > 0.75f)
        {
            timeOfDay = TimeOfDay.Evening;
        }
        else
        {
            timeOfDay = TimeOfDay.Night;
        }

        if (lastTimeOfDay != timeOfDay)
        {
            switch (timeOfDay)
            {
                case TimeOfDay.Morning: 
                    onMorning.Invoke();
                    break;
                case TimeOfDay.Noon:
                    onNoon.Invoke();
                    break;
                case TimeOfDay.Evening:
                    onEvening.Invoke();
                    break;
                case TimeOfDay.Night:
                    onNight.Invoke();
                    break;
            }
        }
    }
}
