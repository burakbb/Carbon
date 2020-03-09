﻿using System.Collections.Generic;

namespace Carbon.MassTransit.Abstractions
{
    public class MassTransitMessage<TContent, TMessage>
    {
        public MassTransitMessage()
        {

        }
        public MassTransitMessage(TContent message)
        {
            Message = message;
            MessageType = new List<string>() { $"urn:message:{typeof(TMessage).FullName.Replace(".", ":")}" };
        }
        public List<string> MessageType { get; set; }
        public TContent Message { get; set; }
    }
}
