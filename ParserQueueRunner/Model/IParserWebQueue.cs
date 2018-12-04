namespace ParserQueueRunner.Model
{
    public interface IParserWebQueue
    {
        ParserQueueElement GetNewElement();
        void SetQueueElementAsProcessed(int ParserQueueId);
    }
}
