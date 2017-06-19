namespace Telemetry
{
    internal interface IChannel
    {
        string Name { get; }

        string Render();

        void Send(object value);

        bool WasUpdated { get; }

    }
}
