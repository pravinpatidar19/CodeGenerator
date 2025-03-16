import React, { useState } from "react";
import axios from "axios";

const PRReview: React.FC = () => {
    const [owner, setOwner] = useState("");
    const [repo, setRepo] = useState("");
    const [prNumber, setPrNumber] = useState("");
    const [review, setReview] = useState("");
    const [loading, setLoading] = useState(false);

    const handleReview = async () => {
        if (!owner || !repo || !prNumber) return;
        setLoading(true);

        try {
            const response = await axios.get("https://localhost:7124/api/pullrequests/review", {
                params: { owner, repo, prNumber },
            });
            setReview(response.data.Review);
        } catch (error) {
            console.error("Error:", error);
            setReview("Failed to get review.");
        }

        setLoading(false);
    };

    return (
        <div className="max-w-xl mx-auto p-4 border rounded shadow">
            <h2 className="text-xl font-bold mb-4">Pull Request Review</h2>

            <input
                type="text"
                placeholder="Owner (e.g., microsoft)"
                value={owner}
                onChange={(e) => setOwner(e.target.value)}
                className="block w-full p-2 border rounded mb-2"
            />
            <input
                type="text"
                placeholder="Repo (e.g., vscode)"
                value={repo}
                onChange={(e) => setRepo(e.target.value)}
                className="block w-full p-2 border rounded mb-2"
            />
            <input
                type="number"
                placeholder="PR Number"
                value={prNumber}
                onChange={(e) => setPrNumber(e.target.value)}
                className="block w-full p-2 border rounded mb-2"
            />
            <button
                onClick={handleReview}
                className="bg-blue-500 text-white px-4 py-2 rounded"
                disabled={loading}
            >
                {loading ? "Reviewing..." : "Review PR"}
            </button>

            {
                review && (
                    <div className="mt-4 p-2 border rounded bg-gray-100">
                        <h3 className="font-bold">AI Review:</h3>
                        <p>{review}</p>
                    </div>
                )
            }
        </div>
    );
};

export default PRReview;
