namespace AHI.Infrastructure.DataCompression.Model
{
    internal class HeldPoint
    {
        public double Angle { get; set; }
        public double MinAngle { get; set; }
        public double MaxAngle { get; set; }

        public HeldPoint(double heldPointAngle, double minAngle, double maxAngle)
        {
            Angle = heldPointAngle;
            MinAngle = minAngle;
            MaxAngle = maxAngle;
        }
    }
}
