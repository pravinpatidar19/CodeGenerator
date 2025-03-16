import axios from 'axios';
import React, { useState } from 'react';
import { useForm } from 'react-hook-form';

interface Message {
    role: "user" | "assistant",
    content: string;
}
function CodeGenerate() {
    const { register, handleSubmit, formState: { errors } } = useForm();
    const [messages, setMessages] = useState<Message[]>([]);

    const onSubmit = async (data: any) => {
        console.log(data);
        const input = data.textArea;
        const userMessage: Message = { role: "user", content: input };
        setMessages((prev) => [...prev, userMessage])
        try {
            const response = await axios.post('https://localhost:7124/api/CodeGenerator/generate', { prompt: input });
            const finalResponse: Message = { role: "assistant", content: response.data };
            setMessages((prev) => [...prev, finalResponse]);
        } catch (error) {
            console.error('Login error:', error);
        }
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <section className="gradient-custom">
                <div className="container py-5">
                    <div className="row">
                        <div className="col-md-12 col-lg-7 col-xl-7">
                            <ul className="list-unstyled mb-0">

                                {
                                    messages.map((msg, index) => (
                                        <li className="d-flex justify-content-between mb-4" key={index}>
                                            <div className="card mask-custom">
                                                <div className="card-header d-flex justify-content-between p-3"
                                                >
                                                    <p className="fw-bold mb-0">{msg.role}</p>
                                                </div>
                                                <div className="card-body">
                                                    <p className="mb-0">
                                                        {msg.content}
                                                    </p>
                                                </div>
                                            </div>
                                        </li>
                                    ))
                                }

                                <li className="mb-3">
                                    <div data-mdb-input-init className="form-outline form-white">
                                        <textarea
                                            id="textArea" className="form-control"
                                            {...register("textArea",
                                                {
                                                    required: "This field is required",
                                                    minLength: {
                                                        value: 10,
                                                        message: "Minimum length should be 10 characters"
                                                    }
                                                })}
                                        ></textarea>
                                        {errors.textArea && <p>{errors.textArea.message?.toString()}</p>}
                                    </div>
                                </li>
                                <button type="submit" data-mdb-button-init data-mdb-ripple-init className="btn btn-light btn-lg btn-rounded float-end">Send</button>
                            </ul>
                        </div>
                    </div>
                </div>
            </section>
        </form>
    );
}

export default CodeGenerate;