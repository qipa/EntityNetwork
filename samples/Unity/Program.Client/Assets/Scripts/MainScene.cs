﻿using UnityEngine;
using System.Collections;
using System.Net;
using Akka.Interfaced;
using Akka.Interfaced.SlimSocket.Base;
using Akka.Interfaced.SlimSocket.Client;
using Common.Logging;
using Domain;
using Domain.Entity;
using TrackableData;
using TypeAlias;

public class MainScene : MonoBehaviour
{
    void Start()
    {
        ClientEntityFactory.Default.RootTransform = GameObject.Find("Canvas").transform;

        var typeTable = new TypeAliasTable();
        typeTable.AddTypeAlias(typeof(ISpaceShip), 12);
        typeTable.AddTypeAlias(typeof(IBullet), 13);
        EntityNetworkManager.TypeTable = typeTable;

        EntityNetworkManager.ProtobufTypeModel = new DomainProtobufSerializer();

        var serializer = new PacketSerializer(
            new PacketSerializerBase.Data(
                new ProtoBufMessageSerializer(new DomainProtobufSerializer()),
                new TypeAliasTable()));

        G.Comm = new Communicator(G.Logger, new IPEndPoint(IPAddress.Loopback, 5000),
            _ => new TcpConnection(serializer, LogManager.GetLogger("Connection")));
        G.Comm.Start();

        StartCoroutine(ProcessTestCounter());
    }

    IEnumerator ProcessTestCounter()
    {
        WriteLine("*** Counter ***");

        var user = new UserRef(new SlimActorRef(1), new SlimRequestWaiter(G.Comm, this), null);

        var t1 = user.GetId();
        yield return t1.WaitHandle;
        ShowResult(t1, "GetId()");

        WriteLine("");

        var t2 = user.EnterGame("Test", 1);
        yield return t2.WaitHandle;
        ShowResult(t2, "GetId()");
    }

    void WriteLine(string text)
    {
        Debug.Log(text);
    }

    void ShowResult(Task task, string name)
    {
        if (task.Status == TaskStatus.RanToCompletion)
            WriteLine(string.Format("{0}: Done", name));
        else if (task.Status == TaskStatus.Faulted)
            WriteLine(string.Format("{0}: Exception = {1}", name, task.Exception));
        else if (task.Status == TaskStatus.Canceled)
            WriteLine(string.Format("{0}: Canceled", name));
        else
            WriteLine(string.Format("{0}: Illegal Status = {1}", name, task.Status));
    }

    void ShowResult<TResult>(Task<TResult> task, string name)
    {
        if (task.Status == TaskStatus.RanToCompletion)
            WriteLine(string.Format("{0}: Result = {1}", name, task.Result));
        else
            ShowResult((Task)task, name);
    }
}