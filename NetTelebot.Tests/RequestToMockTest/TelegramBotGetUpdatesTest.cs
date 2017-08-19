﻿using System;
using System.Linq;
using Mock4Net.Core;
using NetTelebot.Result;
using NetTelebot.Tests.MockServers;
using NetTelebot.Type;
using NUnit.Framework;
using RestSharp;

namespace NetTelebot.Tests.RequestToMockTest
{
    /// <summary>
    /// Testing a chain consisting of private method in <see cref="TelegramBotClient"/> .ExecuteRequest(), 
    /// parser in <see cref="GetUpdatesResult"/> and parser in <see cref="UpdateInfo"/>
    /// 
    /// Server responses are defined within the methods of the current query method is the same (is method GetUpdates <see href="https://core.telegram.org/bots/api#getupdates"/>)
    /// </summary>
    [TestFixture]
    internal class TelegramBotGetUpdatesTest
    {
        private const int mOkServerPort = 8095;
        private const int mBadServerPort = 8096;

        private readonly TelegramBotClient mBotOkResponse = new TelegramBotClient { Token = "Token", RestClient = new RestClient("http://localhost:" + mOkServerPort) };
        private readonly TelegramBotClient mBotBadResponse = new TelegramBotClient { Token = "Token", RestClient = new RestClient("http://localhost:" + mBadServerPort) };

        [OneTimeSetUp]
        public static void OnStart()
        {
            MockServer.Start(mOkServerPort, mBadServerPort);
        }

        [OneTimeTearDown]
        public static void OnStop()
        {
            MockServer.Stop();
        }

        [Test]
        public void GetUpdatesWithMessageObjectTest()
        {
            StartTest(ResponseString.ExpectedBodyForGetUpdatesResultWithObjectMessage);
        }

        [Test]
        public void GetUpdatesWithEditMessageObjectTest()
        {
            StartTest(ResponseString.ExpectedBodyForGetUpdatesResultWithObjectEditMessage);
        }

        [Test]
        public void GetUpdatesWithChannelPostObjectTest()
        {
            StartTest(ResponseString.ExpectedBodyForGetUpdatesResultWithObjectChannelPost);
        }

        private void StartTest(string body)
        {
            MockServer.ServerOkResponse
                .Given(
                    Requests.WithUrl("/botToken/getUpdates").UsingPost()
                )
                .RespondWith(
                    Responses
                        .WithStatusCode(200)
                        .WithBody(body)
                );

            mBotOkResponse.GetUpdates();
            var request = MockServer.ServerOkResponse.SearchLogsFor(Requests.WithUrl("/botToken/getUpdates").UsingPost());
            
            Assert.AreEqual("/botToken/getUpdates", request.FirstOrDefault()?.Url);
            Assert.Throws<Exception>(() => mBotBadResponse.GetUpdates());

        }
    }
}