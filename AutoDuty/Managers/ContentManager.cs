﻿using ECommons;
using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;

namespace AutoDuty.Managers
{
    public class ContentManager
    {
        public List<Content> ListContent { get; set; } = [];

        private List<uint> ListGCArmyContent { get; set; } = [162, 1039, 1041, 1042, 171, 172, 159, 160, 349, 362, 188, 1064, 1066, 430, 510];

        public class Content
        {
            public string? Name { get; set; }

            public uint TerritoryType { get; set; }

            public uint ExVersion { get; set; }

            public byte ClassJobLevelRequired { get; set; }

            public uint ItemLevelRequired { get; set; }

            public bool DawnContent { get; set; } = false;

            public int DawnIndex { get; set; } = -1;

            public bool TrustContent { get; set; } = false;

            public bool GCArmyContent { get; set; } = false;

            public int GCArmyIndex { get; set; } = -1;
        }

        public void PopulateDuties()
        {
            var listContentFinderCondition = Svc.Data.GameData.GetExcelSheet<ContentFinderCondition>();
            var listDawnContent = Svc.Data.GameData.GetExcelSheet<DawnContent>();
            if (listContentFinderCondition == null || listDawnContent == null) return;

            foreach (var contentFinderCondition in listContentFinderCondition)
            {
                if (contentFinderCondition.ContentType.Value == null || contentFinderCondition.TerritoryType.Value == null || contentFinderCondition.TerritoryType.Value.ExVersion.Value == null || contentFinderCondition.ContentType.Value.RowId != 2 ||contentFinderCondition.Name.ToString().IsNullOrEmpty())
                    continue;

                var content = new Content
                {
                    Name = (contentFinderCondition.Name.ToString()[..3].Equals("the") ? contentFinderCondition.Name.ToString().ReplaceFirst("the", "The") : contentFinderCondition.Name.ToString()),
                    TerritoryType = contentFinderCondition.TerritoryType.Value.RowId,
                    ExVersion = contentFinderCondition.TerritoryType.Value.ExVersion.Value.RowId,
                    ClassJobLevelRequired = contentFinderCondition.ClassJobLevelRequired,
                    ItemLevelRequired = contentFinderCondition.ItemLevelRequired,
                    DawnContent = listDawnContent.Any(dawnContent => dawnContent.Content.Value == contentFinderCondition),
                    TrustContent = (listDawnContent.Any(dawnContent => dawnContent.Content.Value == contentFinderCondition) && contentFinderCondition.TerritoryType.Value.ExVersion.Value.RowId > 2),
                    GCArmyContent = ListGCArmyContent.Any(gcArmyContent => gcArmyContent == contentFinderCondition.TerritoryType.Value.RowId),
                    GCArmyIndex = ListGCArmyContent.FindIndex(gcArmyContent => gcArmyContent == contentFinderCondition.TerritoryType.Value.RowId)
                };
                if (content.DawnContent && listDawnContent.Where(dawnContent => dawnContent.Content.Value == contentFinderCondition).Any())
                    content.DawnIndex = listDawnContent.Where(dawnContent => dawnContent.Content.Value == contentFinderCondition).First().RowId < 24 ? (int)listDawnContent.Where(dawnContent => dawnContent.Content.Value == contentFinderCondition).First().RowId : (int)listDawnContent.Where(dawnContent => dawnContent.Content.Value == contentFinderCondition).First().RowId - 200;
                ListContent.Add(content);
            }
            
            ListContent = [.. ListContent.OrderBy(content => content.ExVersion).ThenBy(content => content.ClassJobLevelRequired).ThenBy(content => content.ItemLevelRequired)];
        }
    }
}
