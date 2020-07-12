import React from 'react';

const fetchItems = (baseUrl) => fetch(`${baseUrl}/todos`);

const TodoApp = ({baseUrl}) => {
    const [list, setList] = React.useState([]);
    const [isLoading, setLoading] = React.useState(true);
    
    const setupData = async () => {
        const response = await fetchItems(baseUrl);
        const data = await response.json();
        setList(data);
        setLoading(false);
    };

    React.useEffect(() => {
        setupData(); 
    });
    
    const content = <div>
        <input disabled={isLoading} placeholder="Enter a task name"/>
        <button disabled={isLoading}>Add Task</button>
        <ul>
            {list.map(item => (<li key={item.id}>
                {item.description} -
                {item.isCompleted}
            </li>))}
        </ul>
    </div>;
    
    return (
        <>
            <h3>Todo App</h3>
            {content}
        </>
    );
};

export default TodoApp;