import React, { useState, useEffect, useRef } from "react";
import axios from "axios";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";
import ScrollToBottom from "react-scroll-to-bottom";

interface Message {
    role: "user" | "assistant";
    content: string;
}

const ChatGPTChat: React.FC = () => {
    const [messages, setMessages] = useState<Message[]>([]);
    const [input, setInput] = useState("");
    const [loading, setLoading] = useState(false);
    const messageEndRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        messageEndRef.current?.scrollIntoView({ behavior: "smooth" });
    }, [messages]);

    const handleSend = async () => {
        if (!input.trim()) return;

        const userMessage: Message = { role: "user", content: input };
        setMessages((prev) => [...prev, userMessage]);

        setInput("");
        setLoading(true);

        try {
            const response = await axios.post("https://localhost:7124/api/CodeGenerator/generate", { prompt: input });

            const botMessage: Message = { role: "assistant", content: response.data };
            setMessages((prev) => [...prev, botMessage]);
        } catch (error) {
            console.error("Error:", error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="flex flex-col max-w-lg mx-auto border rounded-lg p-4 shadow-md bg-gray-100 h-[500px]">
            <ScrollToBottom className="h-[400px] overflow-y-auto p-2 bg-white rounded">
                {messages.map((msg, index) => (
                    <div key={index} className={`p-2 m-1 rounded-md ${msg.role === "user" ? "bg-blue-500 text-white self-end text-right" : "bg-gray-300 text-black self-start text-left"}`}>
                        <strong>{msg.role === "user" ? "You" : "ChatGPT"}:</strong>
                        <ReactMarkdown remarkPlugins={[remarkGfm]}>
                            {msg.content}
                        </ReactMarkdown>
                    </div>
                ))}
                <div ref={messageEndRef} />
            </ScrollToBottom>

            <div className="mt-2 flex">
                <input type="text" value={input} onChange={(e) => setInput(e.target.value)} className="flex-grow p-2 border rounded-l-lg" placeholder="Type your message..." />
                <button onClick={handleSend} className="p-2 bg-blue-600 text-white rounded-r-lg" disabled={loading}>
                    {loading ? "Typing..." : "Send"}
                </button>
            </div>
        </div>
    );
};

export default ChatGPTChat;
