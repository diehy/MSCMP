// Generated at 01/02/2025 17:14:22
using System.IO;
using System.Collections.Generic;
namespace MSCMP.Network.Messages {
	public class AnimSyncMessage: INetMessage {
		public byte MessageId {
			get {
				return 18;
			}
		}
		public AnimSyncMessage() {
		}
		public System.Boolean	isRunning;
		public System.Boolean	isLeaning;
		public System.Boolean	isGrounded;
		public System.Byte	activeHandState;
		public System.Single	aimRot;
		public System.Single	crouchPosition;
		public System.Boolean	isDrunk;
		public System.Byte	drinkId;
		public System.Int32	swearId;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Boolean)isRunning);
				writer.Write((System.Boolean)isLeaning);
				writer.Write((System.Boolean)isGrounded);
				writer.Write((System.Byte)activeHandState);
				writer.Write((System.Single)aimRot);
				writer.Write((System.Single)crouchPosition);
				writer.Write((System.Boolean)isDrunk);
				writer.Write((System.Byte)drinkId);
				writer.Write((System.Int32)swearId);
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				isRunning = reader.ReadBoolean();
				isLeaning = reader.ReadBoolean();
				isGrounded = reader.ReadBoolean();
				activeHandState = reader.ReadByte();
				aimRot = reader.ReadSingle();
				crouchPosition = reader.ReadSingle();
				isDrunk = reader.ReadBoolean();
				drinkId = reader.ReadByte();
				swearId = reader.ReadInt32();
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class AskForWorldStateMessage: INetMessage {
		public byte MessageId {
			get {
				return 8;
			}
		}
		public AskForWorldStateMessage() {
		}

		public bool Write(BinaryWriter writer) {
			try {
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class VehicleStateMessage: INetMessage {
		public byte MessageId {
			get {
				return 5;
			}
		}
		public VehicleStateMessage() {
		}
		private byte optionalsMask = 0;
		public System.Int32	objectID;
		public System.Int32	state;
		public System.Int32	dashstate;
		private System.Single	startTime;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Byte)optionalsMask);
				writer.Write((System.Int32)objectID);
				writer.Write((System.Int32)state);
				writer.Write((System.Int32)dashstate);
				if ((optionalsMask & 1) != 0) {
					writer.Write((System.Single)startTime);
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				optionalsMask = reader.ReadByte();
				objectID = reader.ReadInt32();
				state = reader.ReadInt32();
				dashstate = reader.ReadInt32();
				if ((optionalsMask & 1) != 0) {
					startTime = reader.ReadSingle();
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public System.Single StartTime {
			get {
				return startTime;
			}
			set {
				startTime = value;
				optionalsMask |= 1;
			}
		}
		public bool HasStartTime {
			get {
				return (optionalsMask & 1) != 0;
			}
		}
	}
	public class DisconnectMessage: INetMessage {
		public byte MessageId {
			get {
				return 3;
			}
		}
		public DisconnectMessage() {
		}

		public bool Write(BinaryWriter writer) {
			try {
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class EventHookSyncMessage: INetMessage {
		public byte MessageId {
			get {
				return 22;
			}
		}
		public EventHookSyncMessage() {
		}
		private byte optionalsMask = 0;
		public System.Int32	fsmID;
		public System.Int32	fsmEventID;
		public System.Boolean	request;
		private System.String	fsmEventName;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Byte)optionalsMask);
				writer.Write((System.Int32)fsmID);
				writer.Write((System.Int32)fsmEventID);
				writer.Write((System.Boolean)request);
				if ((optionalsMask & 1) != 0) {
					writer.Write((System.String)fsmEventName);
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				optionalsMask = reader.ReadByte();
				fsmID = reader.ReadInt32();
				fsmEventID = reader.ReadInt32();
				request = reader.ReadBoolean();
				if ((optionalsMask & 1) != 0) {
					fsmEventName = reader.ReadString();
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public System.String FsmEventName {
			get {
				return fsmEventName;
			}
			set {
				fsmEventName = value;
				optionalsMask |= 1;
			}
		}
		public bool HasFsmEventName {
			get {
				return (optionalsMask & 1) != 0;
			}
		}
	}
	public class DoorsInitMessage {
		public DoorsInitMessage() {
			position = new Vector3Message();
		}
		public System.Boolean	open;
		public Vector3Message	position;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Boolean)open);
				if (!position.Write(writer)) {
					return false;
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				open = reader.ReadBoolean();
				if (!position.Read(reader)) {
					return false;
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class VehicleInitMessage {
		public VehicleInitMessage() {
			transform = new TransformMessage();
		}
		public System.Byte	id;
		public TransformMessage	transform;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Byte)id);
				if (!transform.Write(writer)) {
					return false;
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				id = reader.ReadByte();
				if (!transform.Read(reader)) {
					return false;
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class FullWorldSyncMessage: INetMessage {
		public byte MessageId {
			get {
				return 7;
			}
		}
		public FullWorldSyncMessage() {
			doors = new DoorsInitMessage[0];
			vehicles = new VehicleInitMessage[0];
			pickupables = new PickupableSpawnMessage[0];
			lights = new LightSwitchMessage[0];
			currentWeather = new WeatherUpdateMessage();
			spawnPosition = new Vector3Message();
			spawnRotation = new QuaternionMessage();
		}
		public System.String	mailboxName;
		public System.Int32	day;
		public System.Single	dayTime;
		public DoorsInitMessage[]	doors;
		public VehicleInitMessage[]	vehicles;
		public PickupableSpawnMessage[]	pickupables;
		public LightSwitchMessage[]	lights;
		public WeatherUpdateMessage	currentWeather;
		public Vector3Message	spawnPosition;
		public QuaternionMessage	spawnRotation;
		public System.Byte	occupiedVehicleId;
		public System.Boolean	passenger;
		public System.UInt16	pickedUpObject;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.String)mailboxName);
				writer.Write((System.Int32)day);
				writer.Write((System.Single)dayTime);
				writer.Write((System.Int32)doors.Length);
				foreach (DoorsInitMessage value in doors) {
					if (!value.Write(writer)) {
						return false;
					}
				}
				writer.Write((System.Int32)vehicles.Length);
				foreach (VehicleInitMessage value in vehicles) {
					if (!value.Write(writer)) {
						return false;
					}
				}
				writer.Write((System.Int32)pickupables.Length);
				foreach (PickupableSpawnMessage value in pickupables) {
					if (!value.Write(writer)) {
						return false;
					}
				}
				writer.Write((System.Int32)lights.Length);
				foreach (LightSwitchMessage value in lights) {
					if (!value.Write(writer)) {
						return false;
					}
				}
				if (!currentWeather.Write(writer)) {
					return false;
				}
				if (!spawnPosition.Write(writer)) {
					return false;
				}
				if (!spawnRotation.Write(writer)) {
					return false;
				}
				writer.Write((System.Byte)occupiedVehicleId);
				writer.Write((System.Boolean)passenger);
				writer.Write((System.UInt16)pickedUpObject);
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				mailboxName = reader.ReadString();
				day = reader.ReadInt32();
				dayTime = reader.ReadSingle();
				System.Int32 doorsLength = reader.ReadInt32();
				doors = new DoorsInitMessage[doorsLength];
				for (int i = 0 ; i < doorsLength; ++i) {
					doors[i] = new DoorsInitMessage();
					if (!doors[i].Read(reader)) {
						return false;
					}
				}
				System.Int32 vehiclesLength = reader.ReadInt32();
				vehicles = new VehicleInitMessage[vehiclesLength];
				for (int i = 0 ; i < vehiclesLength; ++i) {
					vehicles[i] = new VehicleInitMessage();
					if (!vehicles[i].Read(reader)) {
						return false;
					}
				}
				System.Int32 pickupablesLength = reader.ReadInt32();
				pickupables = new PickupableSpawnMessage[pickupablesLength];
				for (int i = 0 ; i < pickupablesLength; ++i) {
					pickupables[i] = new PickupableSpawnMessage();
					if (!pickupables[i].Read(reader)) {
						return false;
					}
				}
				System.Int32 lightsLength = reader.ReadInt32();
				lights = new LightSwitchMessage[lightsLength];
				for (int i = 0 ; i < lightsLength; ++i) {
					lights[i] = new LightSwitchMessage();
					if (!lights[i].Read(reader)) {
						return false;
					}
				}
				if (!currentWeather.Read(reader)) {
					return false;
				}
				if (!spawnPosition.Read(reader)) {
					return false;
				}
				if (!spawnRotation.Read(reader)) {
					return false;
				}
				occupiedVehicleId = reader.ReadByte();
				passenger = reader.ReadBoolean();
				pickedUpObject = reader.ReadUInt16();
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class HandshakeMessage: INetMessage {
		public byte MessageId {
			get {
				return 0;
			}
		}
		public HandshakeMessage() {
		}
		public System.Int32	protocolVersion;
		public System.UInt64	clock;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Int32)protocolVersion);
				writer.Write((System.UInt64)clock);
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				protocolVersion = reader.ReadInt32();
				clock = reader.ReadUInt64();
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class HeartbeatMessage: INetMessage {
		public byte MessageId {
			get {
				return 1;
			}
		}
		public HeartbeatMessage() {
		}
		public System.UInt64	clientClock;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.UInt64)clientClock);
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				clientClock = reader.ReadUInt64();
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class HeartbeatResponseMessage: INetMessage {
		public byte MessageId {
			get {
				return 2;
			}
		}
		public HeartbeatResponseMessage() {
		}
		public System.UInt64	clientClock;
		public System.UInt64	clock;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.UInt64)clientClock);
				writer.Write((System.UInt64)clock);
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				clientClock = reader.ReadUInt64();
				clock = reader.ReadUInt64();
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class LightSwitchMessage: INetMessage {
		public byte MessageId {
			get {
				return 16;
			}
		}
		public LightSwitchMessage() {
			pos = new Vector3Message();
		}
		public Vector3Message	pos;
		public System.Boolean	toggle;

		public bool Write(BinaryWriter writer) {
			try {
				if (!pos.Write(writer)) {
					return false;
				}
				writer.Write((System.Boolean)toggle);
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				if (!pos.Read(reader)) {
					return false;
				}
				toggle = reader.ReadBoolean();
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public enum MessageIds : System.Int32 {
		Handshake = 0,
		Heartbeat = 1,
		HeartbeatResponse = 2,
		Disconnect = 3,
		PlayerSync = 4,
		VehicleState = 5,
		OpenDoors = 6,
		FullWorldSync = 7,
		AskForWorldState = 8,
		VehicleEnter = 9,
		VehicleLeave = 10,
		PickupableSpawn = 11,
		PickupableDestroy = 12,
		PickupableActivate = 13,
		PickupableSetPosition = 14,
		WorldPeriodicalUpdate = 15,
		LightSwitch = 16,
		WeatherSync = 17,
		AnimSync = 18,
		VehicleSwitch = 19,
		ObjectSync = 20,
		ObjectSyncResponse = 21,
		EventHookSync = 22,
		RequestObjectSync = 23,
	}
	public class MessageIdsHelpers {
		public static bool IsValueValid(System.Int32 value) {
			switch (value) {
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
					return true;
			}
			return false;
		}
	}
	public class ObjectSyncMessage: INetMessage {
		public byte MessageId {
			get {
				return 20;
			}
		}
		public ObjectSyncMessage() {
			position = new Vector3Message();
			rotation = new QuaternionMessage();
			syncedVariables = new System.Single[0];
		}
		private byte optionalsMask = 0;
		public System.Int32	objectID;
		public Vector3Message	position;
		public QuaternionMessage	rotation;
		private System.Int32	syncType;
		private System.Single[]	syncedVariables;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Byte)optionalsMask);
				writer.Write((System.Int32)objectID);
				if (!position.Write(writer)) {
					return false;
				}
				if (!rotation.Write(writer)) {
					return false;
				}
				if ((optionalsMask & 1) != 0) {
					writer.Write((System.Int32)syncType);
				}
				if ((optionalsMask & 2) != 0) {
					writer.Write((System.Int32)syncedVariables.Length);
					foreach (System.Single value in syncedVariables) {
						writer.Write((System.Single)value);
					}
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				optionalsMask = reader.ReadByte();
				objectID = reader.ReadInt32();
				if (!position.Read(reader)) {
					return false;
				}
				if (!rotation.Read(reader)) {
					return false;
				}
				if ((optionalsMask & 1) != 0) {
					syncType = reader.ReadInt32();
				}
				if ((optionalsMask & 2) != 0) {
					System.Int32 syncedVariablesLength = reader.ReadInt32();
					syncedVariables = new System.Single[syncedVariablesLength];
					for (int i = 0 ; i < syncedVariablesLength; ++i) {
						syncedVariables[i] = reader.ReadSingle();
					}
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public System.Int32 SyncType {
			get {
				return syncType;
			}
			set {
				syncType = value;
				optionalsMask |= 1;
			}
		}
		public bool HasSyncType {
			get {
				return (optionalsMask & 1) != 0;
			}
		}
		public System.Single[] SyncedVariables {
			get {
				return syncedVariables;
			}
			set {
				syncedVariables = value;
				optionalsMask |= 2;
			}
		}
		public bool HasSyncedVariables {
			get {
				return (optionalsMask & 2) != 0;
			}
		}
	}
	public class ObjectSyncRequestMessage: INetMessage {
		public byte MessageId {
			get {
				return 23;
			}
		}
		public ObjectSyncRequestMessage() {
		}
		public System.Int32	objectID;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Int32)objectID);
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				objectID = reader.ReadInt32();
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class ObjectSyncResponseMessage: INetMessage {
		public byte MessageId {
			get {
				return 21;
			}
		}
		public ObjectSyncResponseMessage() {
		}
		public System.Int32	objectID;
		public System.Boolean	accepted;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Int32)objectID);
				writer.Write((System.Boolean)accepted);
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				objectID = reader.ReadInt32();
				accepted = reader.ReadBoolean();
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class OpenDoorsMessage: INetMessage {
		public byte MessageId {
			get {
				return 6;
			}
		}
		public OpenDoorsMessage() {
			position = new Vector3Message();
		}
		public Vector3Message	position;
		public System.Boolean	open;

		public bool Write(BinaryWriter writer) {
			try {
				if (!position.Write(writer)) {
					return false;
				}
				writer.Write((System.Boolean)open);
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				if (!position.Read(reader)) {
					return false;
				}
				open = reader.ReadBoolean();
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class PickupableActivateMessage: INetMessage {
		public byte MessageId {
			get {
				return 13;
			}
		}
		public PickupableActivateMessage() {
		}
		public System.Int32	id;
		public System.Boolean	activate;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Int32)id);
				writer.Write((System.Boolean)activate);
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				id = reader.ReadInt32();
				activate = reader.ReadBoolean();
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class PickupableDestroyMessage: INetMessage {
		public byte MessageId {
			get {
				return 12;
			}
		}
		public PickupableDestroyMessage() {
		}
		public System.Int32	id;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Int32)id);
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				id = reader.ReadInt32();
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class PickupableSetPositionMessage: INetMessage {
		public byte MessageId {
			get {
				return 14;
			}
		}
		public PickupableSetPositionMessage() {
			position = new Vector3Message();
		}
		public System.Int32	id;
		public Vector3Message	position;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Int32)id);
				if (!position.Write(writer)) {
					return false;
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				id = reader.ReadInt32();
				if (!position.Read(reader)) {
					return false;
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class PickupableSpawnMessage: INetMessage {
		public byte MessageId {
			get {
				return 11;
			}
		}
		public PickupableSpawnMessage() {
			transform = new TransformMessage();
			data = new System.Single[0];
		}
		private byte optionalsMask = 0;
		public System.Int32	id;
		public System.Int32	prefabId;
		public TransformMessage	transform;
		public System.Boolean	active;
		private System.Single[]	data;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Byte)optionalsMask);
				writer.Write((System.Int32)id);
				writer.Write((System.Int32)prefabId);
				if (!transform.Write(writer)) {
					return false;
				}
				writer.Write((System.Boolean)active);
				if ((optionalsMask & 1) != 0) {
					writer.Write((System.Int32)data.Length);
					foreach (System.Single value in data) {
						writer.Write((System.Single)value);
					}
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				optionalsMask = reader.ReadByte();
				id = reader.ReadInt32();
				prefabId = reader.ReadInt32();
				if (!transform.Read(reader)) {
					return false;
				}
				active = reader.ReadBoolean();
				if ((optionalsMask & 1) != 0) {
					System.Int32 dataLength = reader.ReadInt32();
					data = new System.Single[dataLength];
					for (int i = 0 ; i < dataLength; ++i) {
						data[i] = reader.ReadSingle();
					}
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public System.Single[] Data {
			get {
				return data;
			}
			set {
				data = value;
				optionalsMask |= 1;
			}
		}
		public bool HasData {
			get {
				return (optionalsMask & 1) != 0;
			}
		}
	}
	public class PickedUpSync {
		public PickedUpSync() {
			position = new Vector3Message();
			rotation = new QuaternionMessage();
		}
		public Vector3Message	position;
		public QuaternionMessage	rotation;

		public bool Write(BinaryWriter writer) {
			try {
				if (!position.Write(writer)) {
					return false;
				}
				if (!rotation.Write(writer)) {
					return false;
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				if (!position.Read(reader)) {
					return false;
				}
				if (!rotation.Read(reader)) {
					return false;
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class PlayerSyncMessage: INetMessage {
		public byte MessageId {
			get {
				return 4;
			}
		}
		public PlayerSyncMessage() {
			position = new Vector3Message();
			rotation = new QuaternionMessage();
			pickedUpData = new PickedUpSync();
		}
		private byte optionalsMask = 0;
		public Vector3Message	position;
		public QuaternionMessage	rotation;
		private PickedUpSync	pickedUpData;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Byte)optionalsMask);
				if (!position.Write(writer)) {
					return false;
				}
				if (!rotation.Write(writer)) {
					return false;
				}
				if ((optionalsMask & 1) != 0) {
					if (!pickedUpData.Write(writer)) {
						return false;
					}
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				optionalsMask = reader.ReadByte();
				if (!position.Read(reader)) {
					return false;
				}
				if (!rotation.Read(reader)) {
					return false;
				}
				if ((optionalsMask & 1) != 0) {
					if (!pickedUpData.Read(reader)) {
						return false;
					}
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public PickedUpSync PickedUpData {
			get {
				return pickedUpData;
			}
			set {
				pickedUpData = value;
				optionalsMask |= 1;
			}
		}
		public bool HasPickedUpData {
			get {
				return (optionalsMask & 1) != 0;
			}
		}
	}
	public class QuaternionMessage {
		public QuaternionMessage() {
		}
		public System.Single	w;
		public System.Single	x;
		public System.Single	y;
		public System.Single	z;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Single)w);
				writer.Write((System.Single)x);
				writer.Write((System.Single)y);
				writer.Write((System.Single)z);
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				w = reader.ReadSingle();
				x = reader.ReadSingle();
				y = reader.ReadSingle();
				z = reader.ReadSingle();
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class TransformMessage {
		public TransformMessage() {
			position = new Vector3Message();
			rotation = new QuaternionMessage();
		}
		public Vector3Message	position;
		public QuaternionMessage	rotation;

		public bool Write(BinaryWriter writer) {
			try {
				if (!position.Write(writer)) {
					return false;
				}
				if (!rotation.Write(writer)) {
					return false;
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				if (!position.Read(reader)) {
					return false;
				}
				if (!rotation.Read(reader)) {
					return false;
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class Vector3Message {
		public Vector3Message() {
		}
		public System.Single	x;
		public System.Single	y;
		public System.Single	z;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Single)x);
				writer.Write((System.Single)y);
				writer.Write((System.Single)z);
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				x = reader.ReadSingle();
				y = reader.ReadSingle();
				z = reader.ReadSingle();
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class VehicleSwitchMessage: INetMessage {
		public byte MessageId {
			get {
				return 19;
			}
		}
		public VehicleSwitchMessage() {
		}
		private byte optionalsMask = 0;
		public System.Int32	objectID;
		public System.Int32	switchID;
		public System.Boolean	switchValue;
		private System.Single	switchValueFloat;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Byte)optionalsMask);
				writer.Write((System.Int32)objectID);
				writer.Write((System.Int32)switchID);
				writer.Write((System.Boolean)switchValue);
				if ((optionalsMask & 1) != 0) {
					writer.Write((System.Single)switchValueFloat);
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				optionalsMask = reader.ReadByte();
				objectID = reader.ReadInt32();
				switchID = reader.ReadInt32();
				switchValue = reader.ReadBoolean();
				if ((optionalsMask & 1) != 0) {
					switchValueFloat = reader.ReadSingle();
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public System.Single SwitchValueFloat {
			get {
				return switchValueFloat;
			}
			set {
				switchValueFloat = value;
				optionalsMask |= 1;
			}
		}
		public bool HasSwitchValueFloat {
			get {
				return (optionalsMask & 1) != 0;
			}
		}
	}
	public class VehicleEnterMessage: INetMessage {
		public byte MessageId {
			get {
				return 9;
			}
		}
		public VehicleEnterMessage() {
		}
		public System.Int32	objectID;
		public System.Boolean	passenger;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Int32)objectID);
				writer.Write((System.Boolean)passenger);
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				objectID = reader.ReadInt32();
				passenger = reader.ReadBoolean();
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class VehicleLeaveMessage: INetMessage {
		public byte MessageId {
			get {
				return 10;
			}
		}
		public VehicleLeaveMessage() {
		}

		public bool Write(BinaryWriter writer) {
			try {
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class WeatherUpdateMessage: INetMessage {
		public byte MessageId {
			get {
				return 17;
			}
		}
		public WeatherUpdateMessage() {
		}
		public WeatherType	weatherType;
		public System.Single	weatherPos;
		public System.Single	weatherPosSecond;
		public System.Single	weatherOffset;
		public System.Single	weatherRot;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Int32)weatherType);
				writer.Write((System.Single)weatherPos);
				writer.Write((System.Single)weatherPosSecond);
				writer.Write((System.Single)weatherOffset);
				writer.Write((System.Single)weatherRot);
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				System.Int32 _weatherTypeValue = reader.ReadInt32();
				if (!WeatherTypeHelpers.IsValueValid(_weatherTypeValue)) {
					return false;
				}
				weatherType = (WeatherType)_weatherTypeValue;
				weatherPos = reader.ReadSingle();
				weatherPosSecond = reader.ReadSingle();
				weatherOffset = reader.ReadSingle();
				weatherRot = reader.ReadSingle();
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public class WorldPeriodicalUpdateMessage: INetMessage {
		public byte MessageId {
			get {
				return 15;
			}
		}
		public WorldPeriodicalUpdateMessage() {
			currentWeather = new WeatherUpdateMessage();
		}
		public System.Byte	sunClock;
		public System.Byte	worldDay;
		public WeatherUpdateMessage	currentWeather;

		public bool Write(BinaryWriter writer) {
			try {
				writer.Write((System.Byte)sunClock);
				writer.Write((System.Byte)worldDay);
				if (!currentWeather.Write(writer)) {
					return false;
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
		public bool Read(BinaryReader reader) {
			try {
				sunClock = reader.ReadByte();
				worldDay = reader.ReadByte();
				if (!currentWeather.Read(reader)) {
					return false;
				}
				return true;
			}
			catch (System.Exception) {
				return false;
			}
		}
	}
	public enum WeatherType : System.Int32 {
		RAIN = 0,
		THUNDER = 1,
		SUNNY = 2,
	}
	public class WeatherTypeHelpers {
		public static bool IsValueValid(System.Int32 value) {
			switch (value) {
				case 0:
				case 1:
				case 2:
					return true;
			}
			return false;
		}
	}
}

//eof

