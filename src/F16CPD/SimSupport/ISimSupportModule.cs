using Common.Drawing;
using F16CPD.Mfd.Controls;
using F16CPD.Networking;

namespace F16CPD.SimSupport
{
    public interface ISimSupportModule
    {
        bool IsSimRunning { get; }
        bool IsSendingInput { get; }
        F16CpdMfdManager Manager { get; set; }
        void HandleInputControlEvent(CpdInputControls eventSource, MfdInputControl control);
        void UpdateManagerFlightData();
        void InitializeFlightData();
        bool ProcessPendingMessageToClientFromServer(Message message);
        bool ProcessPendingMessageToServerFromClient(Message message);

        void RenderMap(Graphics g, Rectangle renderRect, float mapZoom, int rangeRingRadiusInNauticalMiles,
                       MapRotationMode rotationMode);
    }
}