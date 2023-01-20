﻿using LiturgyGeek.Calendars.Dates;
using LiturgyGeek.Calendars.Model;
using LiturgyGeek.Framework.Clcs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Engine
{
    public class CalendarReader : ICalendarReader
    {
        public ChurchCalendar Read(Stream jsonStream, Stream? lineStream = null)
        {
            var calendar = JsonSerializer.Deserialize<ChurchCalendar>(jsonStream)!;
            if (lineStream != null)
            {
                using (var lineReader  = new StreamReader(lineStream))
                    ApplyLines(calendar, lineReader);
            }
            return calendar;
        }

        public ChurchCalendar Read(string json, string? lines = null)
        {
            var calendar = JsonSerializer.Deserialize<ChurchCalendar>(json)!;
            if (lines != null)
            {
                using (var lineReader = new StreamReader(lines))
                    ApplyLines(calendar, lineReader);
            }
            return calendar;
        }

        private void ApplyLines(ChurchCalendar calendar, TextReader lineReader)
        {
            string? line;
            while ((line = lineReader.ReadLine()) != null)
            {
                calendar.Events.Add(ParseLineEvent(line));
            }
        }

        private ChurchEvent ParseLineEvent(string line)
        {
            var parts = line.Split(new[] { ' ' }, 2);
            if (parts.Length < 2)
                throw new InvalidDataException();

            var result = new ChurchEvent()
            {
                OccasionId = parts[0],
            };

            do
            {
                parts = parts[1].Split(new[] { ' ' }, 2);

                switch (parts[0][0])
                {
                    case '@':
                        result.Dates.Add(ChurchDate.Parse(parts[0].Substring(1)));
                        break;

                    case '$':
                        result.CommonRules.Add(parts[0].Substring(1));
                        break;

                    case '#':
                        result.Flags.Add(parts[0].Substring(1));
                        break;
                }

            } while (parts.Length > 1);

            return result;
        }
    }
}
