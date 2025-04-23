using DBDOverlay.Core.Extensions;

namespace DBDOverlay.Core.Reshade
{
    public class Mapping
    {
        public int MapIndex { get; set; }
        public int FilterIndex { get; set; }
        public string Entry => $"{MapIndex}-{FilterIndex}";

        public Mapping(string entry) 
        {
            var result = entry.Split('-');
            MapIndex = result[0].ToInt();
            FilterIndex = result[1].ToInt();
        }
    }
}
