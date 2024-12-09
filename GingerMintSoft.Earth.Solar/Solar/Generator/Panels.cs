using System.Collections;

namespace GingerMintSoft.Earth.Location.Solar.Generator
{
    public class Panels
    {
        /// <summary>
        /// Anzahl der Solarmodule
        /// </summary>
        public int Count => Read.Count;

        public List<Panel> Read { get; set; } = [];

        public void Add(Panel panel)
        {
            Read.Add(panel);
        }

        public void Remove(Panel panel)
        {
            Read.Remove(panel);
        }

        public void Clear()
        {
            Read.Clear();
        }
    }
}
