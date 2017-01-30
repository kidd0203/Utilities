using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncProgramming
{
    public partial class Form1 : Form
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        #region constructor
        public Form1()
        {
            InitializeComponent();
        }
        #endregion constructor

        #region AwaitScenarios
        private async void btnNormal_Click(object sender, EventArgs e)
        {
            // Run sequentially. start A, finish A, then start B, finish B, then go to end.
            txtResult.Text = "";
            txtResult.Text = "Main Normal starts.";
            await TaskA();
            await TaskB();
            txtResult.Text = txtResult.Text + Environment.NewLine + "Main completes.";
        }
        private void btnNoAwait_Click(object sender, EventArgs e)
        {
            // Run sequentially. start A, start B, then go to end. then A complete, B complete
            txtResult.Text = "";
            txtResult.Text = "Main No Await starts.";
            TaskA();
            TaskB();
            txtResult.Text = txtResult.Text + Environment.NewLine + "Main completes.";
        }
        private async void btnWhenAny_Click(object sender, EventArgs e)
        {
            // Run parallel, start A and B. either complete, go to end.
            txtResult.Text = "";
            txtResult.Text =  "Main When Any starts.";
            List<Task> tasks = new List<Task>();
            tasks.Add(TaskA());
            tasks.Add(TaskB());
            await Task.WhenAny(tasks);
            txtResult.Text = txtResult.Text + Environment.NewLine + "Main completes.";

        }

        private async void btnWhenAll_Click(object sender, EventArgs e)
        {
            // Run parallel, start A and B. both complete, go to end.
            txtResult.Text = "";
            txtResult.Text = "Main When All starts.";
            List<Task> tasks = new List<Task>();
            tasks.Add(TaskA());
            tasks.Add(TaskB());
            await Task.WhenAll(tasks);

            txtResult.Text = txtResult.Text + Environment.NewLine + "Main completes.";
        }

        private void btnWaitAll_Click(object sender, EventArgs e)
        {
            // cause deadlock
            txtResult.Text = "";
            txtResult.Text = "Main Wait All starts.";
            List<Task> tasks = new List<Task>();
            tasks.Add(TaskA());
            tasks.Add(TaskB());
            Task.WaitAll(tasks.ToArray());

            txtResult.Text = txtResult.Text + Environment.NewLine + "Main completes.";
        }

        private void btnWaitAny_Click(object sender, EventArgs e)
        {
            // cause deadlock
            txtResult.Text = "";
            txtResult.Text = "Main Wait Any starts.";
            List<Task> tasks = new List<Task>();
            tasks.Add(TaskA());
            tasks.Add(TaskB());
            Task.WaitAny(tasks.ToArray());
            txtResult.Text = txtResult.Text + Environment.NewLine + "Main completes.";
        }
        #endregion AwaitScenarios

        #region Task

        private async Task TaskA()
        {
            //txtResult.Text = txtResult.Text + Environment.NewLine + "TaskA starts.";
            await Task.Delay(2000);
            txtResult.Text = txtResult.Text + Environment.NewLine + "TaskA completes.";
        }
        private async Task TaskB()
        {
            //txtResult.Text = txtResult.Text + Environment.NewLine + "TaskB starts.";
            await Task.Delay(1000);
            txtResult.Text = txtResult.Text + Environment.NewLine + "TaskB completes.";
        }
        private async Task TaskC()
        {
            //txtResult.Text = txtResult.Text + Environment.NewLine + "TaskB starts.";
            await Task.Delay(3000);
            txtResult.Text = txtResult.Text + Environment.NewLine + "TaskC completes.";
        }

        #endregion Task




        private void btnTaskParallel_Click(object sender, EventArgs e)
        {
            int a = 0;
            int b = 0;
            int c = 0;

            Task A = Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(1000);
                a = a + 10;
            });

            Task B = Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(2000);
                b = b + 20;
            });

            // wait for both A and B complete, then proceed to C.
            Task.WaitAll(A,B);

            Task C = Task.Factory.StartNew(()=>
            {
                System.Threading.Thread.Sleep(2000);
                c = a + b;
            });

            // wait for C complete, then pop out the message.
            // without this Wait(), the message will be popped out immediately after A and B complete.
            C.Wait();

            MessageBox.Show(string.Format("Final result for C: {0}", c.ToString()));
        }

        private void btnTaskParallelNew_Click(object sender, EventArgs e)
        {
            int a = 0;
            int b = 0;
            int c = 0;

            Parallel.Invoke(
                () =>
                {
                    System.Threading.Thread.Sleep(3000);
                    a = a + 10;
                },
                () => {
                    System.Threading.Thread.Sleep(5000);
                    b = b + 20;
                }
                );

            c = a + b;
            MessageBox.Show(string.Format("Final result for C: {0}", c.ToString()));
        }

        private async void btnTaskParallelAwait_Click(object sender, EventArgs e)
        {
            int a = 0;
            int b = 0;
            int c = 0;

            Task A = Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(1000);
                a = a + 10;
            });

            Task B = Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(2000);
                b = b + 20;
            });

            // wait for both A and B complete, then proceed to C.
            await Task.WhenAll(A, B);

            Task C = Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(2000);
                c = a + b;
            });

            await C;
            // wait for C complete, then pop out the message.
            // without this Wait(), the message will be popped out immediately after A and B complete.

            MessageBox.Show(string.Format("Final result for C: {0}", c.ToString()));
        }

        

        private void btnStartAsyncLoop_Click(object sender, EventArgs e)
        {
            CancellationToken token = cts.Token;

            Task A = Task.Factory.StartNew(() =>
            {
                int i = 0;
                while (!token.IsCancellationRequested)
                {
                    System.Threading.Thread.Sleep(1000);
                    i++;
                    //Invoke(new Action(() =>
                    //{
                    //    txtResult.Text = txtResult.Text + i.ToString();
                    //}));
                    //txtResult.Text = txtResult.Text + i.ToString();
                    try
                    {
                        txtResult.Text = txtResult.Text + i.ToString();
                    }
                    catch (Exception ex)
                    {
                        var message = ex.Message;
                    }
                }
            }, token); 
        }

        private void btnCancelAsyncLoop_Click(object sender, EventArgs e)
        {
            cts.Cancel();
        }

        private void btnMessage_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Test message.");
        }

        private void btnTaskFactory_Click(object sender, EventArgs e)
        {
            txtResult.Text = "";
            txtResult.Text += "start";

            Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(2000);

                this.BeginInvoke((Action)(() =>
                {
                    txtResult.Text += "wake up";
                }));
                
            });

            txtResult.Text += "Complete";
        }


        #region TaskWithReturn

        private async Task<string> TaskAA()
        {
            //txtResult.Text = txtResult.Text + Environment.NewLine + "TaskA starts.";
            await Task.Delay(2000);
            //txtResult.Text = txtResult.Text + Environment.NewLine + "TaskA completes.";
            return "AA";
        }
        private async Task<string> TaskBB()
        {
            //txtResult.Text = txtResult.Text + Environment.NewLine + "TaskB starts.";
            await Task.Delay(1000);
            //txtResult.Text = txtResult.Text + Environment.NewLine + "TaskB completes.";
            return "BB";
        }
        private async Task<string> TaskCC()
        {
            //txtResult.Text = txtResult.Text + Environment.NewLine + "TaskB starts.";
            await Task.Delay(3000);
            //txtResult.Text = txtResult.Text + Environment.NewLine + "TaskC completes.";
            return "CC";
        }

        #endregion TaskWithReturn

        private async void button1_Click(object sender, EventArgs e)
        {
            Task<string> A = TaskAA();
            Task<string> B = TaskBB();
            Task<string> C = TaskCC();

            var tasks = new[] { A, B, C };

            //foreach (var t in tasks)
            //{
            //    var result = await t;
            //    txtResult.Text = txtResult.Text + Environment.NewLine + result.ToString();
            //}

            //var processingTasks = (from t in tasks
            //                       select AwaitAndProcessAsync(t)).ToArray();


            var processingTasks = tasks.Select(t => AwaitAndProcessAsync(t)).ToArray();

            //var processingTasks = tasks.Select(async t =>
            //{
            //    var result = await t;
            //    txtResult.Text = txtResult.Text + Environment.NewLine + result.ToString();
            //}).ToArray();

            await Task.WhenAll(processingTasks);
        }

        private async Task AwaitAndProcessAsync(Task<string> t)
        {
            var result = await t;
            txtResult.Text = txtResult.Text + Environment.NewLine + result.ToString();
        }

    }
}
