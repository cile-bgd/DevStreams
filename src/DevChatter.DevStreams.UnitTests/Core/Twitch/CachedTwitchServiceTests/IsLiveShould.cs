﻿using DevChatter.DevStreams.Core.Caching;
using DevChatter.DevStreams.Core.Twitch;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace DevChatter.DevStreams.UnitTests.Core.Twitch.CachedTwitchServiceTests
{
    public class IsLiveShould
    {
        private readonly Mock<ICacheLayer<int, bool>> _cacheMock;
        private readonly Mock<ITwitchService> _twitchMock;
        const int twitchId = 1337;

        public IsLiveShould()
        {
            _cacheMock = new Mock<ICacheLayer<int,bool>>();
            _twitchMock = new Mock<ITwitchService>();
        }

        [Fact]
        public void NotCallTwitch_WhenValueIsCached()
        {
            var twitchService = new CachedTwitchService(_twitchMock.Object, _cacheMock.Object);
            _cacheMock.Setup(x => 
                    x.GetValueOrFallback(twitchId, twitchService.IsLiveFallback))
                .Returns(Task.FromResult(true));

            bool isLive = twitchService.IsLive(twitchId).Result;

            _twitchMock.Verify(x => x.IsLive(twitchId), Times.Never);
        }

        [Fact]
        public void CallTwitch_WhenValueIsNotCached()
        {
            var twitchService = new CachedTwitchService(_twitchMock.Object, _cacheMock.Object);
            _cacheMock.Setup(x =>
                    x.GetValueOrFallback(twitchId, twitchService.IsLiveFallback))
                .Returns(Task.FromResult(false));

            bool isLive = twitchService.IsLive(twitchId).Result;

            _twitchMock.Verify(x => x.IsLive(twitchId), Times.Once);
        }
    }
}