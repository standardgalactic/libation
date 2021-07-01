﻿using Dinah.Core;
using Dinah.Core.Diagnostics;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AaxDecrypter
{
    public class ChapterInfo
    {
        private List<Chapter> _chapterList = new List<Chapter>();
        public IEnumerable<Chapter> Chapters => _chapterList.AsEnumerable();
        public int Count => _chapterList.Count;
        public ChapterInfo() { }
        public ChapterInfo(string audiobookFile) 
        {
            var info = new ProcessStartInfo
            {
                FileName = DecryptSupportLibraries.ffprobePath,
                Arguments = "-loglevel panic -show_chapters -print_format json \"" + audiobookFile + "\""
            };

            var xml = info.RunHidden().Output;
            var chapterJObject = JObject.Parse(xml);
            var chapters = chapterJObject["chapters"]
                .Select(c => new Chapter(
                    c["tags"]?["title"]?.Value<string>(), 
                    c["start_time"].Value<double>(),
                    c["end_time"].Value<double>()
                    ));

            _chapterList.AddRange(chapters);
        }
        public void AddChapter(Chapter chapter)
        {
            ArgumentValidator.EnsureNotNull(chapter, nameof(chapter));
            _chapterList.Add(chapter);
        }
        public string ToFFMeta(bool includeFFMetaHeader)
        {
            var ffmetaChapters = new StringBuilder();

            if (includeFFMetaHeader)
                ffmetaChapters.AppendLine(";FFMETADATA1\n");

            foreach (var c in Chapters)
            {
                ffmetaChapters.AppendLine(c.ToFFMeta());
            }
            return ffmetaChapters.ToString();
        }
    }
    public class Chapter
    {
        public string Title { get; }
        public TimeSpan StartOffset { get; }
        public TimeSpan EndOffset { get; }
        public Chapter(string title, long startOffsetMs, long lengthMs)
        {
            ArgumentValidator.EnsureNotNullOrEmpty(title, nameof(title));
            ArgumentValidator.EnsureGreaterThan(startOffsetMs, nameof(startOffsetMs), -1);
            ArgumentValidator.EnsureGreaterThan(lengthMs, nameof(lengthMs), 0);

            Title = title;
            StartOffset = TimeSpan.FromMilliseconds(startOffsetMs);
            EndOffset = StartOffset +  TimeSpan.FromMilliseconds(lengthMs);
        }
        public Chapter(string title, double startTimeSec, double endTimeSec)
            :this(title, (long)(startTimeSec * 1000), (long)((endTimeSec - startTimeSec) * 1000))
        {
        }

        public string ToFFMeta()
        {
            return "[CHAPTER]\n" +
                "TIMEBASE=1/1000\n" +
                "START=" + StartOffset.TotalMilliseconds + "\n" +
                "END=" + EndOffset.TotalMilliseconds + "\n" +
                "title=" + Title;
        }
    }
}
