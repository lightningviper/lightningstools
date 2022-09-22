using Common.HardwareSupport;
using Common.IO.Ports;
using Common.MacroProgramming;
using log4net;
using SimLinkup.PacketEncoding;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using static SimLinkup.HardwareSupport.ArduinoSeat.ArduinoSeatCommunicationProtocolHeaders;
using SerialPort = Common.IO.Ports.SerialPort;

namespace SimLinkup.HardwareSupport.ArduinoSeat
{
    public class ArduinoSeatHardwareSupportModule : HardwareSupportModuleBase, IDisposable
    {
        private const int BAUD_RATE = 115200;
        private const int DATA_BITS = 8;
        private const Parity PARITY = Parity.None;
        private const StopBits STOP_BITS = StopBits.One;
        private const Handshake HANDSHAKE = Handshake.None;
        private const int WRITE_BUFFER_SIZE = 1024;
        private const int SERIAL_WRITE_TIMEOUT = 500;
        private const int MAX_UPDATE_FREQUENCY_HZ = 30;

        //limits exceptions when we don't have the Arduino plugged into the serial port
        private const int MAX_UNSUCCESSFUL_PORT_OPEN_ATTEMPTS = 5;

        private static readonly ILog _log = LogManager.GetLogger(typeof(ArduinoSeatHardwareSupportModule));

        private ArduinoSeatHardwareSupportModuleConfig _config;
        private readonly object _serialPortLock = new object();
        private bool _isDisposed;

        private ISerialPort _serialPort;
        private int _unsuccessfulConnectionAttempts;
        private byte[] _lastPacketSent = null;
        private DateTime _lastPacketSentTime = DateTime.MinValue;

        private readonly AnalogSignal[] _analogSignals;
        private readonly DigitalSignal[] _digitalSignals;

        private DigitalSignal _gunIsFiring;
        private DigitalSignal _endOfFlight;
        private DigitalSignal _ejecting;
        private DigitalSignal _in3D;
        private DigitalSignal _isPaused;
        private DigitalSignal _isFrozen;
        private DigitalSignal _isOverG;
        private DigitalSignal _isOnGround;
        private DigitalSignal _isExitGame;

        private DigitalSignal _speedBrakeOpen;

        private AnalogSignal _AAMisileFired;
        private AnalogSignal _AGMisileFired;
        private AnalogSignal _bombDropped;
        private AnalogSignal _flareDropped;
        private AnalogSignal _chaffDropped;
        private AnalogSignal _bulletsFired;
        private AnalogSignal _collisionCounter;
        private AnalogSignal _gforces;
        private AnalogSignal _lastDamage;
        private AnalogSignal _damageForce;
        private AnalogSignal _whenDamage;
        private AnalogSignal _bumpIntensity;

        private AnalogSignal _nozzle1Position;
        private AnalogSignal _nozzle2Position;
        private AnalogSignal _rpm1Pct;
        private AnalogSignal _rpm2Pct;
        private AnalogSignal _fuelFlow1;
        private AnalogSignal _fuelFlow2;

        private AnalogSignal _speedBrakePosition;
        private AnalogSignal _climbAngleDegrees;
        private AnalogSignal _sideslipAngleDegrees;
        private AnalogSignal _groundSpeed;
        private AnalogSignal _airSpeed;
        private AnalogSignal _vvi;
        private AnalogSignal _LEFlaps;
        private AnalogSignal _TEFlaps;

        private AnalogSignal _gearPosition;
        private AnalogSignal _noseGearPosition;
        private AnalogSignal _rightGearPosition;
        private AnalogSignal _leftGearPosition;

        private AnalogSignal _bytesSentSignal;

        private FileSystemWatcher _configFileWatcher;
        private bool _configChanged = false;
        private DateTime _lastConfigModified = DateTime.MinValue;

        public ArduinoSeatHardwareSupportModule(ArduinoSeatHardwareSupportModuleConfig config)
        {
            _config = config;
            CreateSignals(out _analogSignals, out _digitalSignals);

            _configFileWatcher = new FileSystemWatcher(Path.GetDirectoryName(_config.FilePath), Path.GetFileName(_config.FilePath));
            _configFileWatcher.Changed += _config_Changed;
            _configFileWatcher.EnableRaisingEvents = true;
        }

        private void _config_Changed(object sender, FileSystemEventArgs e)
        {
            if (_lastConfigModified != File.GetLastWriteTime(_config.FilePath))
            {
                _configChanged = true;
                var configfile = _config.FilePath;
                _config = ArduinoSeatHardwareSupportModuleConfig.Load(configfile);
                _config.FilePath = configfile;
                _lastConfigModified = File.GetLastWriteTime(_config.FilePath);
                _configChanged = false;
            }
        }

        public override AnalogSignal[] AnalogInputs => null;
        public override AnalogSignal[] AnalogOutputs => _analogSignals;
        public override DigitalSignal[] DigitalInputs => _digitalSignals;
        public override DigitalSignal[] DigitalOutputs => null;
        public override TextSignal[] TextInputs => null;
        public override TextSignal[] TextOutputs => null;

        public override string FriendlyName => $"Arduino Seat module on {_config.COMPort}";

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ArduinoSeatHardwareSupportModule()
        {
            Dispose(false);
        }

        public static IHardwareSupportModule[] GetInstances()
        {
            var toReturn = new List<IHardwareSupportModule>();
            try
            {
                var hsmConfigFilePath = Path.Combine(Util.CurrentMappingProfileDirectory, "ArduinoSeatHardwareSupportModule.config");
                var hsmConfig = ArduinoSeatHardwareSupportModuleConfig.Load(hsmConfigFilePath);
                hsmConfig.FilePath = hsmConfigFilePath;
                IHardwareSupportModule thisHsm = new ArduinoSeatHardwareSupportModule(hsmConfig);
                toReturn.Add(thisHsm);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
            return toReturn.ToArray();
        }

        public override void Synchronize()
        {
            base.Synchronize();
            UpdateOutputs();
        }

        private void _serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            _log.ErrorFormat(
                $"A serial port error occurred communicating on COM port {_config.COMPort}.\r\nError Type: {e.EventType}\r\nError Description:{e}");
        }

        private void CreateSignals(out AnalogSignal[] analogSignals, out DigitalSignal[] digitalSignals)
        {
            var digitalSignalsToReturn = new List<DigitalSignal>();

            _gunIsFiring = new DigitalSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Actions",
                FriendlyName = "Gun Is Firing",
                Id = "ArduinoSeat__IS_FIRING_GUN",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = _config.COMPort,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = false
            };
            digitalSignalsToReturn.Add(_gunIsFiring);

            _endOfFlight = new DigitalSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Actions",
                FriendlyName = "End of Flight",
                Id = "ArduinoSeat__IS_END_FLIGHT",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = _config.COMPort,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = false
            };
            digitalSignalsToReturn.Add(_endOfFlight);

            _ejecting = new DigitalSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Actions",
                FriendlyName = "Ejecting",
                Id = "ArduinoSeat__IS_EJECTING",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = _config.COMPort,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = false
            };
            digitalSignalsToReturn.Add(_ejecting);

            _in3D = new DigitalSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Actions",
                FriendlyName = "In 3D",
                Id = "ArduinoSeat__IN_3D",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = _config.COMPort,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = false
            };
            digitalSignalsToReturn.Add(_in3D);

            _isPaused = new DigitalSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Actions",
                FriendlyName = "Is Paused",
                Id = "ArduinoSeat__IS_PAUSED",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = _config.COMPort,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = false
            };
            digitalSignalsToReturn.Add(_isPaused);

            _isFrozen = new DigitalSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Actions",
                FriendlyName = "Is Frozen",
                Id = "ArduinoSeat__IS_FROZEN",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = _config.COMPort,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = false
            };
            digitalSignalsToReturn.Add(_isFrozen);

            _isOverG = new DigitalSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Actions",
                FriendlyName = "Is OverG",
                Id = "ArduinoSeat__IS_OVER_G",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = _config.COMPort,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = false
            };
            digitalSignalsToReturn.Add(_isOverG);

            _isOnGround = new DigitalSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Actions",
                FriendlyName = "Is On Ground",
                Id = "ArduinoSeat__IS_ON_GROUND",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = _config.COMPort,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = false
            };
            digitalSignalsToReturn.Add(_isOnGround);

            _isExitGame = new DigitalSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Actions",
                FriendlyName = "Is Exit Game",
                Id = "ArduinoSeat__IS_EXIT_GAME",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = _config.COMPort,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = false
            };
            digitalSignalsToReturn.Add(_isExitGame);

            _speedBrakeOpen = new DigitalSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Airframe",
                FriendlyName = "Speed Brake Open",
                Id = "ArduinoSeat__SPEED_BRAKE__NOT_STOWED_FLAG",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = _config.COMPort,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = false
            };
            digitalSignalsToReturn.Add(_speedBrakeOpen);

            var analogSignalsToReturn = new List<AnalogSignal>();

            _AAMisileFired = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Ordonnance",
                FriendlyName = "AA Missiles Fired",
                Id = "ArduinoSeat__AA_MISSILE_FIRED",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = int.MaxValue
            };
            analogSignalsToReturn.Add(_AAMisileFired);

            _AGMisileFired = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Ordonnance",
                FriendlyName = "AG Missiles Fired",
                Id = "ArduinoSeat__AG_MISSILE_FIRED",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = int.MaxValue
            };
            analogSignalsToReturn.Add(_AGMisileFired);

            _bombDropped = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Ordonnance",
                FriendlyName = "Bombs Dropped",
                Id = "ArduinoSeat__BOMB_DROPPED",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = int.MaxValue
            };
            analogSignalsToReturn.Add(_bombDropped);

            _flareDropped = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Ordonnance",
                FriendlyName = "Flares Dropped",
                Id = "ArduinoSeat__FLARE_DROPPED",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = int.MaxValue
            };
            analogSignalsToReturn.Add(_flareDropped);

            _chaffDropped = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Ordonnance",
                FriendlyName = "Chaff Dropped",
                Id = "ArduinoSeat__CHAFF_DROPPED",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = int.MaxValue
            };
            analogSignalsToReturn.Add(_chaffDropped);

            _bulletsFired = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Ordonnance",
                FriendlyName = "Bullets Fired",
                Id = "ArduinoSeat__BULLETS_FIRED",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = int.MaxValue
            };
            analogSignalsToReturn.Add(_bulletsFired);

            _collisionCounter = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "External",
                FriendlyName = "Collision Counter",
                Id = "ArduinoSeat__COLLISION_COUNTER",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = int.MaxValue
            };
            analogSignalsToReturn.Add(_collisionCounter);

            _gforces = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "External",
                FriendlyName = "GForces",
                Id = "ArduinoSeat__GFORCE",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 1,
                MinValue = -6,
                MaxValue = 10
            };
            analogSignalsToReturn.Add(_gforces);

            _lastDamage = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "External",
                FriendlyName = "Last Damage",
                Id = "ArduinoSeat__LAST_DAMAGE",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = int.MaxValue
            };
            analogSignalsToReturn.Add(_lastDamage);

            _damageForce = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "External",
                FriendlyName = "Damage Force",
                Id = "ArduinoSeat__DAMAGE_FORCE",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = int.MaxValue
            };
            analogSignalsToReturn.Add(_damageForce);

            _whenDamage = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "External",
                FriendlyName = "When Damage",
                Id = "ArduinoSeat__WHEN_DAMAGE",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = int.MaxValue
            };
            analogSignalsToReturn.Add(_whenDamage);

            _bumpIntensity = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "External",
                FriendlyName = "Bump Intensity",
                Id = "ArduinoSeat__BUMP_INTENSITY",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 1
            };
            analogSignalsToReturn.Add(_bumpIntensity);

            _nozzle1Position = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Engine",
                FriendlyName = "Nozzle 1 Position",
                Id = "ArduinoSeat__NOZ_POS1__NOZZLE_PERCENT_OPEN",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsPercentage = true,
                MinValue = 0,
                MaxValue = 100
            };
            analogSignalsToReturn.Add(_nozzle1Position);

            _nozzle2Position = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Engine",
                FriendlyName = "Nozzle 2 Position",
                Id = "ArduinoSeat__NOZ_POS2__NOZZLE_PERCENT_OPEN",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsPercentage = true,
                MinValue = 0,
                MaxValue = 100
            };
            analogSignalsToReturn.Add(_nozzle2Position);

            _rpm1Pct = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Engine",
                FriendlyName = "RPM 1 Percent",
                Id = "ArduinoSeat__RPM1__RPM_PERCENT",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsPercentage = true,
                MinValue = 0,
                MaxValue = 103
            };
            analogSignalsToReturn.Add(_rpm1Pct);

            _rpm2Pct = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Engine",
                FriendlyName = "RPM 2 Percent",
                Id = "ArduinoSeat__RPM2__RPM_PERCENT",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsPercentage = true,
                MinValue = 0,
                MaxValue = 103
            };
            analogSignalsToReturn.Add(_rpm2Pct);

            _fuelFlow1 = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Engine",
                FriendlyName = "Fuel Flow 1",
                Id = "ArduinoSeat__FUEL_FLOW1__FUEL_FLOW_POUNDS_PER_HOUR",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsPercentage = false,
                MinValue = 0,
                MaxValue = 99900.00
            };
            analogSignalsToReturn.Add(_fuelFlow1);

            _fuelFlow2 = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Engine",
                FriendlyName = "Fuel Flow 2",
                Id = "ArduinoSeat__FUEL_FLOW2__FUEL_FLOW_POUNDS_PER_HOUR",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsPercentage = false,
                MinValue = 0,
                MaxValue = 99900.00
            };
            analogSignalsToReturn.Add(_fuelFlow2);

            _speedBrakePosition = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Flight Dynamics",
                FriendlyName = "Speed Brake Position",
                Id = "ArduinoSeat__SPEED_BRAKE__POSITION",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsPercentage = false,
                MinValue = 0,
                MaxValue = 1
            };
            analogSignalsToReturn.Add(_speedBrakePosition);

            _climbAngleDegrees = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Flight Dynamics",
                FriendlyName = "Climb Angle Degrees",
                Id = "ArduinoSeat__FLIGHT_DYNAMICS__CLIMBDIVE_ANGLE_DEGREES",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsAngle = true,
                IsPercentage = false,
                MinValue = -90,
                MaxValue = 90
            };
            analogSignalsToReturn.Add(_climbAngleDegrees);

            _sideslipAngleDegrees = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Flight Dynamics",
                FriendlyName = "Sideslip Angle Degrees",
                Id = "ArduinoSeat__FLIGHT_DYNAMICS__SIDESLIP_ANGLE_DEGREES",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsAngle = true,
                IsPercentage = false,
                MinValue = -180,
                MaxValue = 180
            };
            analogSignalsToReturn.Add(_sideslipAngleDegrees);

            _groundSpeed = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Flight Dynamics",
                FriendlyName = "Ground Speed",
                Id = "ArduinoSeat__MAP__GROUND_SPEED_KNOTS",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsAngle = false,
                IsPercentage = false,
                MinValue = 0,
                MaxValue = 1000
            };
            analogSignalsToReturn.Add(_groundSpeed);

            _airSpeed = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Flight Dynamics",
                FriendlyName = "True Air Speed Knots",
                Id = "ArduinoSeat__AIRSPEED_MACH_INDICATOR__TRUE_AIRSPEED_KNOTS",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsAngle = false,
                IsPercentage = false,
                MinValue = 0,
                MaxValue = 1700
            };
            analogSignalsToReturn.Add(_airSpeed);

            _vvi = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Airframe",
                FriendlyName = "Vertical Velocity (feet/min)",
                Id = "ArduinoSeat__VVI__VERTICAL_VELOCITY_FPM",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsAngle = false,
                IsPercentage = false,
                MinValue = -6000,
                MaxValue = 6000
            };
            analogSignalsToReturn.Add(_vvi);

            _LEFlaps = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Airframe",
                FriendlyName = "Leading edge flaps position",
                Id = "ArduinoSeat__AIRCRAFT__LEADING_EDGE_FLAPS_POSITION",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsAngle = false,
                IsPercentage = true,
                MinValue = 0,
                MaxValue = 1
            };
            analogSignalsToReturn.Add(_LEFlaps);

            _TEFlaps = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Flight Dynamics",
                FriendlyName = "Trailing edge flaps position",
                Id = "ArduinoSeat__AIRCRAFT__TRAILING_EDGE_FLAPS_POSITION",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsAngle = false,
                IsPercentage = true,
                MinValue = 0,
                MaxValue = 1
            };
            analogSignalsToReturn.Add(_TEFlaps);
            
            _gearPosition = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Gear",
                FriendlyName = "Gear Position",
                Id = "ArduinoSeat__GEAR_PANEL__GEAR_POSITION",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsAngle = false,
                IsPercentage = true,
                MinValue = 0,
                MaxValue = 1
            };
            analogSignalsToReturn.Add(_gearPosition);

            _noseGearPosition = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Gear",
                FriendlyName = "Nose Gear Position",
                Id = "ArduinoSeat__GEAR_PANEL__NOSE_GEAR_POSITION",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsAngle = false,
                IsPercentage = true,
                MinValue = 0,
                MaxValue = 1
            };
            analogSignalsToReturn.Add(_noseGearPosition);


            _leftGearPosition = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Gear",
                FriendlyName = "Left Gear Position",
                Id = "ArduinoSeat__GEAR_PANEL__LEFT_GEAR_POSITION",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsAngle = false,
                IsPercentage = true,
                MinValue = 0,
                MaxValue = 1
            };
            analogSignalsToReturn.Add(_leftGearPosition);

            _rightGearPosition = new AnalogSignal
            {
                Category = "Inputs from Simulation",
                CollectionName = "Gear",
                FriendlyName = "Right Gear Position",
                Id = "ArduinoSeat__GEAR_PANEL__RIGHT_GEAR_POSITION",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                IsAngle = false,
                IsPercentage = true,
                MinValue = 0,
                MaxValue = 1
            };
            analogSignalsToReturn.Add(_rightGearPosition);

            _bytesSentSignal = new AnalogSignal
            {
                Category = "Performance Tracking",
                CollectionName = "Data Throughput",
                FriendlyName = "# of Bytes Sent (last packet)",
                Id = "ArduinoSeat__Bytes_Sent_Last_Packet",
                Index = 0,
                PublisherObject = this,
                Source = this,
                SourceFriendlyName = FriendlyName,
                SourceAddress = null,
                SubSource = null,
                SubSourceFriendlyName = null,
                SubSourceAddress = null,
                State = 0,
                MinValue = 0,
                MaxValue = 1024
            };
            analogSignalsToReturn.Add(_bytesSentSignal);

            analogSignals = analogSignalsToReturn.ToArray();
            digitalSignals = digitalSignalsToReturn.ToArray();
        }

        private void UpdateOutputs()
        {
            if (_configChanged)
                return;

            if (DateTime.Now.Subtract(_lastPacketSentTime).TotalMilliseconds < (1000 / MAX_UPDATE_FREQUENCY_HZ))
            {
                return;
            }
            var connected = EnsureSerialPortConnected();
            if (!connected) return;

            DateTime currentTime = DateTime.Now;

            byte motorBits = 0;
            byte[] motorSpeeds = new byte[4];

            if (!_isExitGame.State && !_isFrozen.State && !_isPaused.State && !_endOfFlight.State && _in3D.State)
            {
                if (_gunIsFiring.State)
                    SetMotorOutputs(_gunIsFiring.Id, _gunIsFiring.State ? 1 : 0, ref motorBits, ref motorSpeeds);

                if (_bumpIntensity.State > 0)
                    SetMotorOutputs(_bumpIntensity.Id, _bumpIntensity.State, ref motorBits, ref motorSpeeds);

                if (_nozzle1Position.State > 0)
                    SetMotorOutputs(_nozzle1Position.Id, _nozzle1Position.State, ref motorBits, ref motorSpeeds);
            }

            byte[] packet = null;
            using (var stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    stream.WriteByte((byte)ArduinoSeatPacketFields.MOTOR_STATES);
                    stream.WriteByte(motorBits);

                    stream.WriteByte((byte)ArduinoSeatPacketFields.MOTOR_1_SPEED);
                    stream.WriteByte(motorSpeeds[0]);

                    stream.WriteByte((byte)ArduinoSeatPacketFields.MOTOR_2_SPEED);
                    stream.WriteByte(motorSpeeds[1]);

                    stream.WriteByte((byte)ArduinoSeatPacketFields.MOTOR_3_SPEED);
                    stream.WriteByte(motorSpeeds[2]);

                    stream.WriteByte((byte)ArduinoSeatPacketFields.MOTOR_4_SPEED);
                    stream.WriteByte(motorSpeeds[3]);

                    writer.Flush();
                    stream.Flush();
                    packet = stream.ToArray();
                }
            }

            SendPacket(packet);
            _lastPacketSentTime = DateTime.Now;
        }

        private void SetMotorOutputs(string id, double value, ref byte motorbits, ref byte[] motorSpeed)
        {
            var output = _config.SeatOutputs.Where(c => c.ID == id).FirstOrDefault();
            if (output != null)
            {
                if (output.MOTOR_1)
                {
                    motorbits |= _config.MotorByte1;
                    var speed = CalcMotorSpeed(output, value, output.MOTOR_1_SPEED);
                    if (speed > motorSpeed[0])
                        motorSpeed[0] = speed;
                }
            
                if (output.MOTOR_2)
                {
                    motorbits |= _config.MotorByte2;
                    var speed = CalcMotorSpeed(output, value, output.MOTOR_2_SPEED);
                    if (speed > motorSpeed[1])
                        motorSpeed[1] = speed;
                }
                if (output.MOTOR_3)
                {
                    motorbits |= _config.MotorByte3;
                    var speed = CalcMotorSpeed(output, value, output.MOTOR_3_SPEED);
                    if (speed > motorSpeed[2])
                        motorSpeed[2] = speed;
                }
                if (output.MOTOR_4)
                {
                    motorbits |= _config.MotorByte4;
                    var speed = CalcMotorSpeed(output, value, output.MOTOR_4_SPEED);
                    if (speed > motorSpeed[3])
                        motorSpeed[3] = speed;
                }
            }
        }

        private byte CalcMotorSpeed(SeatOutput output, double simValue, byte maxMotorSpeed)
        {
            byte motorSpeed = 0;

            if (simValue >= output.MIN && simValue <= output.MAX)
            {
                switch (output.TYPE)
                {
                    case PulseType.Fixed:
                        if (output.FORCE == MotorForce.Manual)
                            motorSpeed = output.MOTOR_1_SPEED;
                        else
                        {
                            switch (output.FORCE)
                            {
                                case MotorForce.Off:
                                    motorSpeed = 0;
                                    break;
                                case MotorForce.Rumble:
                                    motorSpeed = _config.ForceRumble;
                                    break;
                                case MotorForce.Medium:
                                    motorSpeed = _config.ForceMedium;
                                    break;
                                case MotorForce.Hard:
                                    motorSpeed = _config.ForceHard;
                                    break;
                            }
                        }
                        break;
                    case PulseType.Progressive:
                        {
                            var totalDelta = output.MAX - output.MIN;
                            if (totalDelta > 0)
                            {
                                double calculatedSpeed = ((simValue - output.MIN) / totalDelta) * maxMotorSpeed;
                                motorSpeed = (byte)calculatedSpeed;
                            }
                        }
                        break;
                    case PulseType.CenterPeak:
                        {
                            var totalDelta = (output.MAX - output.MIN) / 2;
                            if (totalDelta > 0)
                            {
                                if (simValue <= output.MIN + totalDelta)
                                {
                                    double calculatedSpeed = ((simValue - output.MIN) / totalDelta) * maxMotorSpeed;
                                    motorSpeed = (byte)calculatedSpeed;
                                }
                                else
                                {
                                    double calculatedSpeed = (totalDelta / (simValue - output.MIN)) * maxMotorSpeed;
                                    motorSpeed = (byte)calculatedSpeed;
                                }
                            }
                        }
                        break;
                }
            }

            return motorSpeed;
        }

        private void SendPacket(byte[] packet)
        {
            lock (_serialPortLock)
            {
                try
                {
                    if (_serialPort == null || !_serialPort.IsOpen || packet == null || packet.Length == 0
                        || (_lastPacketSent != null && packet.SequenceEqual(_lastPacketSent))
                        ) return;
                    var cobsEncodedPacket = COBS.Encode(packet);
                    _serialPort.Write(cobsEncodedPacket, offset: 0, count: cobsEncodedPacket.Length);
                    _serialPort.Write(new byte[] { 0 }, 0, 1);
                    _serialPort.BaseStream.Flush();
                    _lastPacketSent = packet;
                    _bytesSentSignal.State = cobsEncodedPacket.Length;

                }
                catch (Exception e)
                {
                    _log.Error(e.Message, e);
                }
            }
        }

        private bool EnsureSerialPortConnected()
        {
            lock (_serialPortLock)
            {
                if (_serialPort == null)
                {
                    try
                    {
                        _serialPort = new SerialPort(_config.COMPort, BAUD_RATE, PARITY, DATA_BITS, STOP_BITS);
                    }
                    catch (Exception e)
                    {
                        _log.Error(e.Message, e);
                        return false;
                    }
                }
                if (_serialPort != null && !_serialPort.IsOpen &&
                    _unsuccessfulConnectionAttempts < MAX_UNSUCCESSFUL_PORT_OPEN_ATTEMPTS)
                {
                    try
                    {
                        _serialPort.Handshake = HANDSHAKE;
                        _serialPort.WriteTimeout = SERIAL_WRITE_TIMEOUT;
                        _serialPort.ErrorReceived += _serialPort_ErrorReceived;
                        _serialPort.WriteBufferSize = WRITE_BUFFER_SIZE;
                        _log.DebugFormat(
                            $"Opening serial port {_config.COMPort}: Handshake:{HANDSHAKE}, WriteTimeout:{SERIAL_WRITE_TIMEOUT}, WriteBufferSize:{WRITE_BUFFER_SIZE}");
                        _serialPort.Open();
                        GC.SuppressFinalize(_serialPort.BaseStream);
                        _serialPort.DiscardOutBuffer();
                        _unsuccessfulConnectionAttempts = 0;
                        return true;
                    }
                    catch (Exception e)
                    {
                        _unsuccessfulConnectionAttempts++;
                        _log.Error(e.Message, e);
                    }
                }
                else if (_serialPort != null && _serialPort.IsOpen)
                {
                    return true;
                }
                return false;
            }
        }

        private void CloseSerialPortConnection()
        {
            lock (_serialPortLock)
            {
                if (_serialPort != null)
                {
                    try
                    {
                        if (_serialPort.IsOpen)
                        {
                            _log.DebugFormat($"Closing serial port {_config.COMPort}");
                            _serialPort.DiscardOutBuffer();
                            _serialPort.Close();
                        }
                        try
                        {
                            GC.ReRegisterForFinalize(_serialPort.BaseStream);
                        }
                        catch { }
                        _serialPort.Dispose();
                    }
                    catch (Exception e)
                    {
                        _log.Error(e.Message, e);
                    }
                    _serialPort = null;
                }
                _unsuccessfulConnectionAttempts = 0; //reset unsuccessful connection attempts counter
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                try
                {
                    CloseSerialPortConnection();
                }
                catch (Exception e)
                {
                    _log.Error(e.Message, e);
                }
            }
            _isDisposed = true;
        }
    }
}
