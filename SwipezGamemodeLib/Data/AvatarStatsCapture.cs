using SLZ.VRMK;

namespace SwipezGamemodeLib.Data
{
    public class AvatarStatsCapture
    {
        public float speedMult;
        public float agilityMult;
        public float strengthLowerMult;
        public float strengthUpperMult;
        
        public void Apply(Avatar avatar)
        {
            avatar._speed *= speedMult;
            avatar._agility *= agilityMult;
            avatar._strengthLower *= strengthLowerMult;
            avatar._strengthUpper *= strengthUpperMult;
        }
    }
}