import { useState } from 'react';
import './App.css';
import React from 'react';

function App() {
    const [email, setEmail] = useState("");
    const [file, setFile] = useState<File | undefined>();
    const [error, setError] = React.useState({
        emailError: '',
        fileError: ''
    })

    function isValidEmail(email) {
        return /\S+@\S+\.\S+/.test(email);
    }

    function handleOnChange(e: React.FormEvent<HTMLInputElement>) {
        const target = e.target as HTMLInputElement & {
            files: FileList;
        }
        setFile(target.files[0]);
        setError(state => ({ ...state, fileError: '' }));
    }

    const handleSubmit = (e) => {
        e.preventDefault();
        if (file !== undefined && isValidEmail(email)) {
            setError({ emailError: '', fileError: '' });
            const formData = new FormData();
            formData.append('file', file);
            formData.append('email', email);
            fetch('api/item', {
                method: 'POST',
                body: formData
            }).then((response) => {
                if (response.ok) { alert('Item added successfully'); }
                else { alert('Error adding item'); }
            });
        }
        else {
            if (file === undefined) {
                setError(state => ({ ...state, fileError: 'Select a file' }));
            }
            if (!isValidEmail(email)) {
                setError(state => ({ ...state, emailError: 'Invalid email' }));
            }
            return;
        }
    }
    
    return (
        <div>
            <form onSubmit={handleSubmit} encType="multipart/form-data">
                <label htmlFor="f">Please, select a file (.docx only):</label>
                <input type="file" id="f" name="f" onChange={handleOnChange}
                    accept=".docx"></input>
                {error.fileError && <h2 style={{ color: 'red' }}>{error.fileError}</h2>}
                <label htmlFor="em">Please, enter your email</label>
                <input type="text" id="em" name="em" value={email}
                    onChange={(e) => setEmail(e.target.value)}></input>
                {error.emailError && <h2 style={{ color: 'red' }}>{error.emailError}</h2>}
                <button type="submit">Send</button>
            </form>
        </div>
    );

}

export default App;