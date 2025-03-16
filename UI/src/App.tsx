import React from 'react';
import './App.css';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import CodeGenerate from './pages/CodeGenerate';
import Layout from './components/Layout';
import ChatGPTChat from './pages/ChatGPTChat';
import PRReview from './pages/PRReview';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route path="code-generator" element={<CodeGenerate />} />
          <Route path="chat-gpt-chat" element={<ChatGPTChat />} />
          <Route path="pr-review" element={<PRReview />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
