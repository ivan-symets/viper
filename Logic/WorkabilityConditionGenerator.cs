using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VIPER.Presenters;

namespace VIPER.Logic
{
    class WorkabilityConditionGenerator
    {
        public List<Segment> Segments { get; set; } = new List<Segment>();
        public WorkabilityConditionGenerator(List<Segment> segments)
        {
            Segments = segments;
        }

        //метод визначення умови працездатності системи
        public string GenerateCondition()
        {
            var currentSegments = this.Segments;

            //головний цикл з використанням алгоритмів послідовного і паралельного злиття
            do
            {
                // злиття паралельних сегментів системи
                currentSegments = MergeConsistentSegments(currentSegments);

                // злиття послідовних сегментів системи
                currentSegments = MergeParallelSegments(currentSegments);
            } while (currentSegments.Count != 1);

            string condition = ($"({currentSegments[0].SubCondition})");
            Lines.LineBuilders.Clear();

            ConfigurationHelper.CountGetFormula = 0;


            return condition;
        }

        //метод видалення порожніх вузлів
        List<Segment> MergeEmptySegments(List<Segment> segments)
        {
            var emptySegmentIds = new List<int>();

            // знаходження вузлів, які мають порожню  умову працездатності і їх об*єднання з суміжними
            foreach (var emptySegment in segments.Where(w => !Convert.ToBoolean(w.Modules.Count)))
            {
                foreach (var segment in segments.Where(w => Convert.ToBoolean(w.Modules.Count)))
                {

                    if (emptySegment.StartNode == segment.EndNode)
                    {
                        segment.EndNode = emptySegment.EndNode;
                        emptySegmentIds.Add(emptySegment.Id);
                    }

                    if (emptySegment.EndNode == segment.StartNode)
                    {
                        segment.StartNode = emptySegment.StartNode;
                        emptySegmentIds.Add(emptySegment.Id);
                    }
                }

            }
            return segments.Where(w => !emptySegmentIds.Contains(w.Id)).ToList();
        }
        //алгоритм послідовного злиття елементів
        List<Segment> MergeConsistentSegments(List<Segment> segments)
        {
            var mergedSegmentIds = new List<int>();
            segments = segments.OrderBy(o => o.Id).ToList();
            foreach (var currentSegment in segments)
            {
                foreach (var loopSegment in segments)
                {
                    var isUniqueInNode = segments.Where(w => w.StartNode == currentSegment.EndNode).Count() == 1;

                    var isUniqueOutNode = segments.Where(w => w.EndNode == loopSegment.StartNode).Count() == 1;

                    // якщо EndNode поточного елемента дорівнює StartNode наступного і вони зустрічаються по одному разі в масиві сегментів, то потрібно об*єднати сегменти послідовно

                    if (currentSegment.Id < loopSegment.Id && isUniqueOutNode && isUniqueInNode && currentSegment.EndNode == loopSegment.StartNode)
                    {
                        String[] expressionAnd = { "AND" };
                        String[] expressionOr = { "OR" };

                        var subCondition1 = currentSegment.SubCondition;
                        var subCondition2 = loopSegment.SubCondition;

                        currentSegment.Modules.Union(loopSegment.Modules);
                        currentSegment.EndNode = loopSegment.EndNode;

                        //додавання індексв елемента який вже був використаний
                        mergedSegmentIds.Add(loopSegment.Id);


                        if (String.IsNullOrEmpty(subCondition1))
                        {
                            currentSegment.SubCondition = subCondition2;
                            continue;
                        }
                        else if (String.IsNullOrEmpty(subCondition2))
                        {
                            currentSegment.SubCondition = subCondition1;
                            continue;
                        }

                        //перевірка на попередню операцію, якщо вона змінилась потрібно додати дужки
                        if (subCondition1.Split(expressionOr, System.StringSplitOptions.RemoveEmptyEntries).Count() > 1)
                            subCondition1 = $"({subCondition1})";

                        if (subCondition2.Split(expressionOr, System.StringSplitOptions.RemoveEmptyEntries).Count() > 1)
                            subCondition2 = $"({subCondition2})";

                        currentSegment.SubCondition = $"{(subCondition1)} {LogicOperation.AND} {subCondition2}";

                        if (subCondition1 == "")
                            currentSegment.SubCondition = subCondition2;

                        if (subCondition2 == "")
                            currentSegment.SubCondition = subCondition1;


                    }
                }
            }
            return segments.Where(w => !mergedSegmentIds.Contains(w.Id)).ToList();
        }
        //алгоритм паралельного злиття елементів
        List<Segment> MergeParallelSegments(List<Segment> segments)
        {
            var mergedSegmentIds = new List<int>();

            segments = segments.OrderBy(o => o.Id).ToList();

            foreach (var currentSegment in segments)
            {
                foreach (var loopSegment in segments)
                {
                    //якщо існують два або більше сегментів з однаковими початковими і кінцевими вузлами, то треба злити такі вузли паралельно
                    if (currentSegment.Id < loopSegment.Id && currentSegment.StartNode == loopSegment.StartNode && currentSegment.EndNode == loopSegment.EndNode)
                    {
                        String[] expressionAnd = { "AND" };
                        String[] expressionOr = { "OR" };

                        var subCondition1 = currentSegment.SubCondition;
                        var subCondition2 = loopSegment.SubCondition;



                        currentSegment.Modules.Union(loopSegment.Modules);

                        //перевірка на попередню операцію, якщо вона змінилась потрібно додати дужки
                        if (subCondition1.Split(expressionAnd, System.StringSplitOptions.RemoveEmptyEntries).Count() > 1)
                            subCondition1 = $"({subCondition1})";

                        if (subCondition2.Split(expressionAnd, System.StringSplitOptions.RemoveEmptyEntries).Count() > 1)
                            subCondition2 = $"({subCondition2})";

                        currentSegment.SubCondition = $"{subCondition1} {LogicOperation.OR} {subCondition2}";

                        if (subCondition1 == "")
                            currentSegment.SubCondition = subCondition2;

                        if (subCondition2 == "")
                            currentSegment.SubCondition = subCondition1;

                        //додавання індексв елемента який вже був використаний
                        mergedSegmentIds.Add(loopSegment.Id);
                    }
                }
            }
            return segments.Where(w => !mergedSegmentIds.Contains(w.Id)).ToList();
        }
    }
}

