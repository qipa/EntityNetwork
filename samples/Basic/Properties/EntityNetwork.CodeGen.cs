// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Akka.Interfaced CodeGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityNetwork;
using TypeAlias;
using ProtoBuf;
using TrackableData;
using System.Reflection;
using System.Runtime.Serialization;
using ProtoBuf;
using TypeAlias;
using System.ComponentModel;

#region IBullet

namespace Basic
{
    [PayloadTableForEntity(typeof(IBullet))]
    public static class IBullet_PayloadTable
    {
        public static Type[] GetPayloadTypes()
        {
            return new Type[]
            {
                typeof(Hit_Invoke),
            };
        }

        [ProtoContract, TypeAlias]
        public class Hit_Invoke : IInvokePayload
        {
            [ProtoMember(1), DefaultValue(0f)] public float x = 0f;
            [ProtoMember(2), DefaultValue(0f)] public float y = 0f;

            public PayloadFlags Flags { get { return 0; } }

            public void InvokeServer(IEntityServerHandler target)
            {
                ((IBulletServerHandler)target).OnHit(x, y);
            }

            public void InvokeClient(IEntityClientHandler target)
            {
                ((IBulletClientHandler)target).OnHit(x, y);
            }
        }
    }

    public interface IBulletServerHandler : IEntityServerHandler
    {
        void OnHit(float x = 0f, float y = 0f);
    }

    public abstract class BulletServerBase : ServerEntity
    {
        public override int TrackableDataCount { get { return 0; } }

        public override ITrackable GetTrackableData(int index)
        {
            return null;
        }

        public override void SetTrackableData(int index, ITrackable trackable)
        {
        }

        public void Hit(float x = 0f, float y = 0f)
        {
            var payload = new IBullet_PayloadTable.Hit_Invoke { x = x, y = y };
            SendInvoke(payload);
        }
    }

    public interface IBulletClientHandler : IEntityClientHandler
    {
        void OnHit(float x = 0f, float y = 0f);
    }

    public abstract class BulletClientBase : ClientEntity
    {
        public override int TrackableDataCount { get { return 0; } }

        public override ITrackable GetTrackableData(int index)
        {
            return null;
        }

        public override void SetTrackableData(int index, ITrackable trackable)
        {
        }

        public void Hit(float x = 0f, float y = 0f)
        {
            var payload = new IBullet_PayloadTable.Hit_Invoke { x = x, y = y };
            SendInvoke(payload);
        }
    }
}

#endregion

#region ISpaceShip

namespace Basic
{
    [PayloadTableForEntity(typeof(ISpaceShip))]
    public static class ISpaceShip_PayloadTable
    {
        public static Type[] GetPayloadTypes()
        {
            return new Type[]
            {
                typeof(Hit_Invoke),
                typeof(Move_Invoke),
                typeof(Say_Invoke),
                typeof(Shoot_Invoke),
                typeof(Stop_Invoke),
            };
        }

        [ProtoContract, TypeAlias]
        public class Hit_Invoke : IInvokePayload
        {
            [ProtoMember(1), DefaultValue(0f)] public float x = 0f;
            [ProtoMember(2), DefaultValue(0f)] public float y = 0f;

            public PayloadFlags Flags { get { return PayloadFlags.ToServer; } }

            public void InvokeServer(IEntityServerHandler target)
            {
            }

            public void InvokeClient(IEntityClientHandler target)
            {
                ((ISpaceShipClientHandler)target).OnHit(x, y);
            }
        }

        [ProtoContract, TypeAlias]
        public class Move_Invoke : IInvokePayload
        {
            [ProtoMember(1)] public float x;
            [ProtoMember(2)] public float y;
            [ProtoMember(3)] public float dx;
            [ProtoMember(4)] public float dy;

            public PayloadFlags Flags { get { return PayloadFlags.PassThrough; } }

            public void InvokeServer(IEntityServerHandler target)
            {
            }

            public void InvokeClient(IEntityClientHandler target)
            {
                ((ISpaceShipClientHandler)target).OnMove(x, y, dx, dy);
            }
        }

        [ProtoContract, TypeAlias]
        public class Say_Invoke : IInvokePayload
        {
            [ProtoMember(1)] public string msg;

            public PayloadFlags Flags { get { return 0; } }

            public void InvokeServer(IEntityServerHandler target)
            {
                ((ISpaceShipServerHandler)target).OnSay(msg);
            }

            public void InvokeClient(IEntityClientHandler target)
            {
                ((ISpaceShipClientHandler)target).OnSay(msg);
            }
        }

        [ProtoContract, TypeAlias]
        public class Shoot_Invoke : IInvokePayload
        {
            [ProtoMember(1)] public float x;
            [ProtoMember(2)] public float y;
            [ProtoMember(3)] public float dx;
            [ProtoMember(4)] public float dy;

            public PayloadFlags Flags { get { return PayloadFlags.ToClient; } }

            public void InvokeServer(IEntityServerHandler target)
            {
                ((ISpaceShipServerHandler)target).OnShoot(x, y, dx, dy);
            }

            public void InvokeClient(IEntityClientHandler target)
            {
            }
        }

        [ProtoContract, TypeAlias]
        public class Stop_Invoke : IInvokePayload
        {
            [ProtoMember(1)] public float x;
            [ProtoMember(2)] public float y;

            public PayloadFlags Flags { get { return PayloadFlags.PassThrough; } }

            public void InvokeServer(IEntityServerHandler target)
            {
            }

            public void InvokeClient(IEntityClientHandler target)
            {
                ((ISpaceShipClientHandler)target).OnStop(x, y);
            }
        }
    }

    public interface ISpaceShipServerHandler : IEntityServerHandler
    {
        void OnSay(string msg);
        void OnShoot(float x, float y, float dx, float dy);
    }

    public abstract class SpaceShipServerBase : ServerEntity
    {
        public TrackableSpaceShipData Data { get; set; }

        protected SpaceShipServerBase()
        {
            Data = new TrackableSpaceShipData();
        }

        public override object Snapshot { get { return OnSnapshot(); } }

        public abstract SpaceShipSnapshot OnSnapshot();

        public override int TrackableDataCount { get { return 1; } }

        public override ITrackable GetTrackableData(int index)
        {
            if (index == 0) return Data;
            return null;
        }

        public override void SetTrackableData(int index, ITrackable trackable)
        {
            if (index == 0) Data = (TrackableSpaceShipData)trackable;
        }

        public void Hit(float x = 0f, float y = 0f)
        {
            var payload = new ISpaceShip_PayloadTable.Hit_Invoke { x = x, y = y };
            SendInvoke(payload);
        }

        public void Move(float x, float y, float dx, float dy)
        {
            var payload = new ISpaceShip_PayloadTable.Move_Invoke { x = x, y = y, dx = dx, dy = dy };
            SendInvoke(payload);
        }

        public void Say(string msg)
        {
            var payload = new ISpaceShip_PayloadTable.Say_Invoke { msg = msg };
            SendInvoke(payload);
        }

        public void Stop(float x, float y)
        {
            var payload = new ISpaceShip_PayloadTable.Stop_Invoke { x = x, y = y };
            SendInvoke(payload);
        }
    }

    public interface ISpaceShipClientHandler : IEntityClientHandler
    {
        void OnHit(float x = 0f, float y = 0f);
        void OnMove(float x, float y, float dx, float dy);
        void OnSay(string msg);
        void OnStop(float x, float y);
    }

    public abstract class SpaceShipClientBase : ClientEntity
    {
        public TrackableSpaceShipData Data { get; set; }

        protected SpaceShipClientBase()
        {
            Data = new TrackableSpaceShipData();
        }

        public override object Snapshot { set { OnSnapshot((SpaceShipSnapshot)value); } }

        public abstract void OnSnapshot(SpaceShipSnapshot snapshot);

        public override int TrackableDataCount { get { return 1; } }

        public override ITrackable GetTrackableData(int index)
        {
            if (index == 0) return Data;
            return null;
        }

        public override void SetTrackableData(int index, ITrackable trackable)
        {
            if (index == 0) Data = (TrackableSpaceShipData)trackable;
        }

        public void Move(float x, float y, float dx, float dy)
        {
            var payload = new ISpaceShip_PayloadTable.Move_Invoke { x = x, y = y, dx = dx, dy = dy };
            SendInvoke(payload);
        }

        public void Say(string msg)
        {
            var payload = new ISpaceShip_PayloadTable.Say_Invoke { msg = msg };
            SendInvoke(payload);
        }

        public void Shoot(float x, float y, float dx, float dy)
        {
            var payload = new ISpaceShip_PayloadTable.Shoot_Invoke { x = x, y = y, dx = dx, dy = dy };
            SendInvoke(payload);
        }

        public void Stop(float x, float y)
        {
            var payload = new ISpaceShip_PayloadTable.Stop_Invoke { x = x, y = y };
            SendInvoke(payload);
        }
    }
}

#endregion
