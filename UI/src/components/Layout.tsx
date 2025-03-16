import { Outlet, Link } from "react-router-dom";

const Layout = () => {
    return (
        <>
            <nav>
                <ul>
                    <li>
                        <Link to="/">Home</Link>
                    </li>
                    <li>
                        <Link to="/code-generator">Code Generator</Link>
                    </li>
                    <li>
                        <Link to="/pr-review">PR Review</Link>
                    </li>
                    {/* <li>
                        <Link to="/chat-gpt-chat">Chat GPT Chat</Link>
                    </li> */}
                </ul>
            </nav>

            <Outlet />
        </>
    )
};

export default Layout;