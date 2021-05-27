namespace Screens.CalibrationScreen {
    public interface IFocusStateMeasurement {
        void Initialize();
        bool MyUpdate();

        void SetNextState();

        IFocusStateMeasurement Transition();
    }
}