using System;
using System.Collections.Generic;
using System.Linq;

namespace HypeRate.SDK
{
    public class Channels
    {
        private readonly Dictionary<int, string> _joiningChannels = new Dictionary<int, string>();

        private readonly List<string> _joinedChannels = new List<string>();

        private readonly Dictionary<int, string> _leavingChannels = new Dictionary<int, string>();

        private readonly Random _randomNumberGenerator = new Random();

        private readonly List<int> _refsInUse = new List<int>();

        private int GenerateRandomRef()
        {
            var generatedRef = _randomNumberGenerator.Next(1, int.MaxValue - 1);

            while (_refsInUse.Contains(generatedRef))
            {
                generatedRef = _randomNumberGenerator.Next(1, int.MaxValue - 1);
            }

            return generatedRef;
        }

        public RefType DetermineRefType(int @ref)
        {
            if (_joiningChannels.ContainsKey(@ref))
            {
                return RefType.Join;
            }

            return _leavingChannels.ContainsKey(@ref) ? RefType.Leave : RefType.Unknown;
        }

        /// <summary>
        /// Returns -1 when the given channel name has already been joined or is about to be joined.
        /// Removes the channel name from the leaving channels when the channel name is about to be left.
        /// </summary>
        /// <param name="channelName">The name of the channel to join</param>
        /// <returns>
        /// Returns -1 when the given channel name has already been joined or is about to be joined.
        /// Otherwise returns the generated ref for the join command.
        /// </returns>
        public int AddJoiningChannel(string channelName)
        {
            if (_joinedChannels.Contains(channelName))
            {
                return -1;
            }

            if (_joiningChannels.ContainsValue(channelName))
            {
                return -1;
            }

            if (_leavingChannels.ContainsValue(channelName))
            {
                var leavingChannelRef = _leavingChannels.First((pair) => pair.Value == channelName).Key;
                _leavingChannels.Remove(leavingChannelRef);
                _refsInUse.Remove(leavingChannelRef);
            }

            var generatedRef = GenerateRandomRef();

            _joiningChannels.Add(generatedRef, channelName);
            _refsInUse.Add(generatedRef);

            return generatedRef;
        }

        public void HandleChannelJoin(int joinedRef)
        {
            if (_refsInUse.Contains(joinedRef) == false)
            {
                return;
            }

            var channelName = _joiningChannels[joinedRef];

            if (channelName == null)
            {
                return;
            }

            _refsInUse.Remove(joinedRef);
            _joiningChannels.Remove(joinedRef);
            _joinedChannels.Add(channelName);
        }

        public int AddLeavingChannel(string channelName)
        {
            var generatedRef = GenerateRandomRef();

            _leavingChannels.Add(generatedRef, channelName);
            _refsInUse.Add(generatedRef);

            return generatedRef;
        }

        public List<string> GetChannelsToJoin()
        {
            var channelsToLeave = _leavingChannels.Values.ToList();

            var result = _joinedChannels.Where(joinedChannel => !channelsToLeave.Contains(joinedChannel)).ToList();
            result.AddRange(_joiningChannels.Values.Where(joiningChannel => !channelsToLeave.Contains(joiningChannel)));

            return result;
        }

        public void HandleJoin(int @ref)
        {
            if (_refsInUse.Contains(@ref) == false)
            {
                return;
            }

            var joinedChannelName = _joiningChannels[@ref];

            _refsInUse.Remove(@ref);
            _joiningChannels.Remove(@ref);
            _joinedChannels.Add(joinedChannelName);
        }

        public void HandleLeave(int @ref)
        {
            if (_refsInUse.Contains(@ref) == false)
            {
                return;
            }

            var leftChannelName = _leavingChannels[@ref];

            if (leftChannelName == null)
            {
                return;
            }

            _refsInUse.Remove(@ref);
            _leavingChannels.Remove(@ref);
        }

        public void HandleReconnect()
        {
            _joiningChannels.Clear();
            _leavingChannels.Clear();
            _joinedChannels.Clear();
            _refsInUse.Clear();
        }

        public override string ToString()
        {
            return
                $"Channels {{ joining: [ {string.Join(", ", _joiningChannels.Values)} ], joined: [ {string.Join(", ", _joinedChannels)} ], leaving: [ {string.Join(", ", _leavingChannels.Values)} ]}}";
        }
    }
}