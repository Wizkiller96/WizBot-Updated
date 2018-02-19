﻿using Discord.Commands;
using WizBot.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WizBot.Common.Attributes;

namespace WizBot.Modules.Utility
{
    public partial class Utility
    {
        [Group]
        public class CalcCommands : WizBotSubmodule
        {
            [WizBotCommand, Usage, Description, Aliases]
            public async Task Calculate([Remainder] string expression)
            {
                var expr = new NCalc.Expression(expression, NCalc.EvaluateOptions.IgnoreCase);
                expr.EvaluateParameter += Expr_EvaluateParameter;
                var result = expr.Evaluate();
                if (expr.Error == null)
                    await Context.Channel.SendConfirmAsync("⚙ " + GetText("result"), result.ToString());
                else
                    await Context.Channel.SendErrorAsync("⚙ " + GetText("error"), expr.Error);
            }

            private static void Expr_EvaluateParameter(string name, NCalc.ParameterArgs args)
            {
                switch (name.ToLowerInvariant())
                {
                    case "pi":
                        args.Result = Math.PI;
                        break;
                    case "e":
                        args.Result = Math.E;
                        break;
                    default:
                        break;
                }
            }

            [WizBotCommand, Usage, Description, Aliases]
            public async Task CalcOps()
            {
                var selection = typeof(Math).GetTypeInfo()
                    .GetMethods()
                    .Distinct(new MethodInfoEqualityComparer())
                    .Select(x => x.Name)
                    .Except(new[]
                    {
                        "ToString",
                        "Equals",
                        "GetHashCode",
                        "GetType"
                    });
                await Context.Channel.SendConfirmAsync(GetText("calcops", Prefix), string.Join(", ", selection));
            }
        }

        private class MethodInfoEqualityComparer : IEqualityComparer<MethodInfo>
        {
            public bool Equals(MethodInfo x, MethodInfo y) => x.Name == y.Name;

            public int GetHashCode(MethodInfo obj) => obj.Name.GetHashCode();
        }
    }
}