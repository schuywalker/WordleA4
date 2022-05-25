﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wordle.Api.Controllers;
using Wordle.Api.Data;
using Wordle.Api.Services;

namespace Wordle.Api.Tests;
[TestClass]
public class DailyWordTests : DatabaseBaseTests
{
    [TestMethod]
    public void GetDailyWord()
    {
        using var context = new TestAppDbContext(Options);
        Word.SeedWords(context);
        var sut = new GameService(context);


        Word? word1 = sut.GetDailyWord(new DateTime(2020, 1, 1));
        Assert.IsNotNull(word1);
        Assert.AreEqual<int>(5, word1.Value.Length);
        Word? word2 = sut.GetDailyWord(new DateTime(2020, 1, 1));
        Assert.IsNotNull(word2);
        Assert.AreEqual<string?>(word1.Value, word2.Value);
    }

    [TestMethod]
    public void GetDailyGame()
    {
        using var context = new TestAppDbContext(Options);
        Word.SeedWords(context);
        var sut = new GameService(context);

        Guid playerGuid = Guid.NewGuid();
        Game? game = sut.CreateGame(playerGuid, Game.GameTypeEnum.WordOfTheDay, new DateTime(2020, 1, 1));
        Assert.IsNotNull(game);
        Assert.IsNotNull(game.Word);
        Assert.IsNotNull(game.Word.Value);
    }

    [TestMethod]
    public void GetDailyGameNoDateFails()
    {
        using var context = new TestAppDbContext(Options);
        Word.SeedWords(context);
        var sut = new GameService(context);

        Guid playerGuid = Guid.NewGuid();
        Assert.ThrowsException<ArgumentException>(() => 
            sut.CreateGame(playerGuid, Game.GameTypeEnum.WordOfTheDay)
        );

    }

    [Ignore("Needs a bit more work to get the same game to come back.")]
    [TestMethod]
    public void GetDailyGameThatIsFinished()
    {
        using var context = new TestAppDbContext(Options);
        Word.SeedWords(context);
        var sut = new GameService(context);

        Guid playerGuid = Guid.NewGuid();
        Game? game1 = sut.CreateGame(playerGuid, Game.GameTypeEnum.WordOfTheDay, new DateTime(2020, 1, 1));
        sut.FinishGame(game1.GameId);
        Game? game2 = sut.CreateGame(playerGuid, Game.GameTypeEnum.WordOfTheDay, new DateTime(2020, 1, 1));
        Assert.AreEqual(game1.GameId, game2.GameId);
        Assert.IsNotNull(game2.Word.Value);
        Assert.IsNotNull(game2.DateEnded);
    }


}
