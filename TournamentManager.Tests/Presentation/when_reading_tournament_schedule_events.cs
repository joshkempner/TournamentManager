using System;
using System.Linq;
using ReactiveDomain;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using Splat;
using TournamentManager.Domain;
using TournamentManager.Presentation;
using Xunit;

namespace TournamentManager.Tests.Presentation
{
    public class when_reading_tournament_schedule_events : IDisposable
    {
        private readonly MockRepositorySpecification _fixture = new MockRepositorySpecification();
        private readonly TournamentScheduleRM _rm;
        private readonly Guid _tournamentId = Guid.NewGuid();
        private readonly DateTime _tournamentDate = DateTime.Today;
        private readonly CorrelatedStreamStoreRepository _repo;

        public when_reading_tournament_schedule_events()
        {
            Locator.CurrentMutable.RegisterConstant(_fixture.StreamStoreConnection, typeof(IStreamStoreConnection));

            _rm = new TournamentScheduleRM(_tournamentId);

            _repo = new CorrelatedStreamStoreRepository(_fixture.Repository);
            // Add a tournament
            var tourney1 = new Tournament(
                                _tournamentId,
                                "Tourney 1",
                                _tournamentDate,
                                _tournamentDate,
                                MessageBuilder.New(() => new TestCommands.Command1()));
            _repo.Save(tourney1);
        }

        public void Dispose()
        {
            _rm?.Dispose();
        }

        [Fact]
        public void can_see_new_game_slot()
        {
            var tourney = _repo.GetById<Tournament>(_tournamentId, MessageBuilder.New(() => new TestCommands.Command1()));
            var slotId = Guid.NewGuid();
            var startTime = _tournamentDate + TimeSpan.FromHours(9);
            var endTime = _tournamentDate + TimeSpan.FromHours(10);
            tourney.AddGameSlot(
                slotId,
                startTime,
                endTime);
            _repo.Save(tourney);

            AssertEx.IsOrBecomesTrue(() => _rm.GameSlots.Count == 1);
            var slot = _rm.GameSlots.Items.First();
            Assert.Equal(slotId, slot.Id);
            Assert.Equal(startTime, slot.StartTime);
            Assert.Equal(endTime, slot.EndTime);
        }

        [Fact]
        public void can_see_new_field_in_each_game_slot()
        {
            var tourney = _repo.GetById<Tournament>(_tournamentId, MessageBuilder.New(() => new TestCommands.Command1()));
            var slot1Id = Guid.NewGuid();
            var slot2Id = Guid.NewGuid();
            var startTime = _tournamentDate + TimeSpan.FromHours(9);
            var endTime = startTime + TimeSpan.FromMinutes(50);
            var fieldId = Guid.NewGuid();
            const string fieldName = "Field 1";
            tourney.AddGameSlot(
                slot1Id,
                startTime,
                endTime);
            tourney.AddGameSlot(
                slot2Id,
                startTime + TimeSpan.FromHours(1),
                endTime + TimeSpan.FromHours(1));
            tourney.AddField(
                fieldId,
                fieldName);
            _repo.Save(tourney);

            AssertEx.IsOrBecomesTrue(() => _rm.GameSlots.Count == 2);
            AssertEx.IsOrBecomesTrue(() => _rm.GameSlots.Items.ElementAt(0).Games.Count == 1);
            AssertEx.IsOrBecomesTrue(() => _rm.GameSlots.Items.ElementAt(1).Games.Count == 1);

            var gameInSlot1 = _rm.GameSlots.Items.ElementAt(0).Games.Items.First();
            Assert.Equal(fieldId, gameInSlot1.FieldId);
            Assert.Equal(fieldName, gameInSlot1.FieldName);

            var gameInSlot2 = _rm.GameSlots.Items.ElementAt(1).Games.Items.First();
            Assert.Equal(fieldId, gameInSlot2.FieldId);
            Assert.Equal(fieldName, gameInSlot2.FieldName);
        }
    }
}
